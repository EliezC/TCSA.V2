﻿using Microsoft.EntityFrameworkCore;
using System.Data;
using TCSA.V2.Data;
using TCSA.V2.Helpers;
using TCSA.V2.Models;

namespace TCSA.V2.Services;

public interface IPeerReviewService
{
    Task MarkCodeReviewAsCompleted(string reviewerId, int dashboardProjectId, string userId, int currentReviewersPoints);
    Task<List<DashboardProject>> GetProjectsForPeerReview(string userId);
    Task AssignUserToCodeReview(string userId, int id);
    string GetRevieweeName(string revieweeId);
    Task<ApplicationUser> GetUserForPeerReview(string reviewerId);
}
public class PeerReviewService : IPeerReviewService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;
    private readonly ILogger<UserActivityService> _logger;

    public PeerReviewService(ILogger<UserActivityService> logger, IDbContextFactory<ApplicationDbContext> factory)
    {
        _factory = factory;
        _logger = logger;
    }

    public async Task MarkCodeReviewAsCompleted(string reviewerId, int dashboardProjectId, string userId, int currentReviewersPoints)
    {
        try
        {
            using (var context = _factory.CreateDbContext())
            {
                var reviewee = await context.Users.FirstOrDefaultAsync(x => x.Id == userId);
                var currentPoints = reviewee.ExperiencePoints;

                var dashboardProject = await context.DashboardProjects.FirstOrDefaultAsync(x => x.Id == dashboardProjectId);

                var academyProject = ProjectHelper.GetProjects().FirstOrDefault(x => x.Id == dashboardProject.ProjectId);

                dashboardProject.IsPendingReview = false;
                dashboardProject.IsPendingNotification = true;
                dashboardProject.IsCompleted = true;

                context.DashboardProjects
                    .Update(dashboardProject);

                context.UserActivity.AddRange(new AppUserActivity
                {
                    ProjectId = dashboardProject.ProjectId,
                    AppUserId = dashboardProject.AppUserId,
                    DateSubmitted = DateTime.UtcNow,
                    ActivityType = ActivityType.ProjectCompleted
                },
                new AppUserActivity
                {
                    ProjectId = dashboardProject.ProjectId,
                    AppUserId = reviewerId,
                    DateSubmitted = DateTime.UtcNow,
                    ActivityType = ActivityType.CodeReviewCompleted
                });

                context.Users
                    .Where(x => x.Id == dashboardProject.AppUserId)
                    .ExecuteUpdate(y => y.SetProperty(u => u.ExperiencePoints, academyProject.ExperiencePoints + currentPoints));

                context.Users
                    .Where(x => x.Id == reviewerId)
                    .ExecuteUpdate(y => y.SetProperty(u => u.ExperiencePoints, academyProject.ExperiencePoints + currentReviewersPoints));

                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in {nameof(MarkCodeReviewAsCompleted)}");
        }
    }

    public async Task<List<DashboardProject>> GetProjectsForPeerReview(string userId)
    {
        var url = "https://github.com/TheCSharpAcademy/CodeReviews";
        var beginnerProjects = new List<int> { 53, 11, 12, 13 };

        try
        {
            using (var context = _factory.CreateDbContext())
            {
                var reviewProjects = context.UserReviews
                    .Where(x => x.AppUserId != userId)
                    .Select(x => x.DashboardProjectId)
                    .ToList();

                return await context.DashboardProjects
                .AsSplitQuery()
                .Include(x => x.AppUser)
                .Where(x => x.IsPendingReview
                   && beginnerProjects.Contains(x.ProjectId)
                   && !reviewProjects.Contains(x.Id)
                   && x.GithubUrl.StartsWith(url))
                .OrderBy(x => x.DateSubmitted)
                .ToListAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in {nameof(GetProjectsForPeerReview)}");
            return null;
        }
    }

    public async Task AssignUserToCodeReview(string userId, int id)
    {
        using (var context = _factory.CreateDbContext())
        {
            await context.UserReviews.AddAsync(new UserReview
            {
                AppUserId = userId,
                DashboardProjectId = id
            });

            await context.SaveChangesAsync();
        }
    }

    public string GetRevieweeName(string revieweeId)
    {
        try
        {
            using (var context = _factory.CreateDbContext())
            {
                var reviewee = context.AspNetUsers
                    .Where(x => x.Id.Equals(revieweeId))
                    .Select(x => new
                    {
                        DisplayName = string.IsNullOrEmpty(x.DisplayName) ? x.FirstName + " " + x.LastName : x.DisplayName
                    })
                    .FirstOrDefault();

                return reviewee?.DisplayName;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in {nameof(GetRevieweeName)}");
            return null;
        }
    }

    public async Task<ApplicationUser> GetUserForPeerReview(string reviewerId)
    {
        try
        {
            using (var context = _factory.CreateDbContext())
            {
                return await context.AspNetUsers
                    .Where(x => x.Id.Equals(reviewerId))
                    .Include(x => x.CodeReviewProjects)
                    .Select(x => new ApplicationUser
                    {
                        ExperiencePoints = x.ExperiencePoints
                    })
                    .FirstOrDefaultAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in {nameof(GetRevieweeName)}");
            return null;
        }
    }
}
