﻿using TCSA.V2.Helpers.ProjectsSubHelpers;
using TCSA.V2.Models;

namespace TCSA.V2.Helpers;

public static class ProjectHelper
{
    public static List<Project> GetProjects()
    {
        var projects = new List<Project>();
        
        return projects
            .Concat(StandAloneProjectsHelper.GetProjects()) 
            .Concat(ConsoleProjectsHelper.GetProjects())
            .Concat(StartApplyingProjectsHelper.GetProjects())
            .Concat(MVCProjectsHelper.GetProjects())
            .Concat(AngularProjectsHelper.GetProjects())
            .Concat(BlazorProjectsHelper.GetProjects())
            .Concat(ReactProjectsHelper.GetProjects())
            .Concat(SqlProjectsHelper.GetProjects())
            .Concat(AuthProjectsHelper.GetProjects())
            .Concat(AzureProjectsHelper.GetProjects())
            .ToList();  
    }

    public static List<Project> GetProjectsInPortuguese()
    {
        var projects = new List<Project>();

        return projects
            .Concat(StandAloneProjectsHelper.GetProjectsInPortuguese())
            .ToList();
    }
    
    public static List<Project> GetProjectsInDutch()
    {
        var projects = new List<Project>();

        return projects
            .Concat(StandAloneProjectsHelper.GetProjectsInDutch())
            .Concat(ConsoleProjectsHelper.GetProjectsInDutch())
            .ToList();
    }

    public static List<Project> GetProjectsInTurkish()
    {
        var projects = new List<Project>();

        return projects
            .Concat(StandAloneProjectsHelper.GetProjectsInTurkish())
            .ToList();
    }

    public static List<Project> GetProjectsInKorean()
    {
        var projects = new List<Project>();
        return projects
            .Concat(StandAloneProjectsHelper.GetProjectsInKorean())
            .ToList();
    }

    public static List<Project> GetProjectsInTraditionalChinese()
    {
        var projects = new List<Project>();
        return projects
            .Concat(StandAloneProjectsHelper.GetProjectsInTraditionalChinese())
            .ToList();
    }

    public static List<Project> GetProjectsInCroatian()
    {
        var projects = new List<Project>();

        return projects
            .Concat(StandAloneProjectsHelper.GetProjectsInCroatian())
            .ToList();
    }
}
