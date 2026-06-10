using System;
using FlatLife.Controllers;
using FlatLife.Database.ApplicationDbContext;
using FlatLife.Database.Entities;
using FlatLife.Models.FlatDTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace FlatLife.Services;

public class FlatTaskService
{
    private readonly ApplicationDbContext _db;
    private readonly FlatTaskResponseBodyMapper _flatTaskResponseBodyMapper;
    private readonly TaskAssignmentManager _taskAssignmentManager;
    private readonly PayloadReader _payloadReader;


    public FlatTaskService(ApplicationDbContext db, FlatTaskResponseBodyMapper flatTaskResponseBodyMapper, TaskAssignmentManager taskAssignmentManager, PayloadReader payloadReader)
    {
        _db = db;
        _flatTaskResponseBodyMapper = flatTaskResponseBodyMapper;
        _taskAssignmentManager = taskAssignmentManager;
        _payloadReader = payloadReader;
    }


    public async Task<FlatTaskResponseBody> CreateTask(FlatTaskRequestBody flatTaskRequestBody, int activeFlatId)
    {
        try
        {

            var task = await _flatTaskResponseBodyMapper.Map(flatTaskRequestBody, activeFlatId);

            var firstTask = await _db.FlatTask
                   .Include(t => t.CurrentUser)
                   .Where(t => t.FlatId == activeFlatId)
                   .FirstOrDefaultAsync();

            if (firstTask == null && flatTaskRequestBody.Frequency == null)
            {
                throw new ArgumentException("You should add a frequency");
            }

            if (string.IsNullOrEmpty(flatTaskRequestBody.TaskName))
            {
                throw new ArgumentException("Task name cannot be empty");
            }

            if (flatTaskRequestBody.Frequency != null)
            {
                SetTaskFrequency(flatTaskRequestBody.Frequency, activeFlatId);
            }

            await _db.FlatTask.AddAsync(task);
            await _db.SaveChangesAsync();

            return _flatTaskResponseBodyMapper.Map(task);
        }
        catch (DbUpdateException dbEx)
        {
            throw new ArgumentException($"Database error: {dbEx.Message}");

        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Error: {ex.Message}");

        }
    }

    public async Task<FlatTaskResponseBody> DeleteTask(int taskId)
    {
        try
        {
            var task = _db.FlatTask.Include(t => t.CurrentUser).FirstOrDefault(t => t.Id == taskId);

            if (task == null)
            {
                throw new ArgumentException($"Task with ID {taskId} not found.");
            }

            _db.FlatTask.Remove(task);
            await _db.SaveChangesAsync();

            return _flatTaskResponseBodyMapper.Map(task);
        }
        catch (DbUpdateException dbEx)
        {
            throw new ArgumentException($"Database error: {dbEx.Message}");

        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Error: {ex.Message}");

        }

    }

    public async Task<FlatTaskResponseBody> UpdateTask(int taskId, FlatTaskPutBody flatTaskPutBody, int activeFlatId)
    {
        try
        {
            var task = await _db.FlatTask.Include(t => t.CurrentUser).FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null)
            {
                throw new ArgumentException($"Task with ID {taskId} not found.");
            }
            if (!String.IsNullOrEmpty(flatTaskPutBody.TaskName))
            {
                task.TaskName = flatTaskPutBody.TaskName;
            }


            if (flatTaskPutBody.Frequency != null)
            {
                SetTaskFrequency(flatTaskPutBody.Frequency, activeFlatId);
                task.Frequency = (TimeSpan)flatTaskPutBody.Frequency;
            }


            _db.FlatTask.Update(task);
            await _db.SaveChangesAsync();

            return _flatTaskResponseBodyMapper.Map(task);
        }
        catch (DbUpdateException dbEx)
        {
            throw new ArgumentException($"Database error: {dbEx.Message}");

        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Error: {ex.Message}");

        }
    }


    public void SetTaskFrequency(TimeSpan? newFrequency, int activeFlatId)
    {
        if (newFrequency <= TimeSpan.Zero)
        {
            throw new ArgumentException("The interval should be more than 0");
        }
        var tasksInFlat = _db.FlatTask.Where(t => t.FlatId == activeFlatId).ToList();

        foreach (var task in tasksInFlat)
        {
            task.Frequency = (TimeSpan)newFrequency;
        }

        _taskAssignmentManager.DefaultFrequency = (TimeSpan)newFrequency;
        _taskAssignmentManager.NextChangeDate = DateTime.UtcNow.Add((TimeSpan)newFrequency);
    }



    public void TaskRotation()
    {
        var currentTime = DateTime.UtcNow;
        DateTime earliestNextRotation = DateTime.MaxValue;

        var tasksByFlat = _db.FlatTask
            .Include(t => t.CurrentUser)
            .GroupBy(t => t.FlatId)
            .ToList();

        foreach (var flatTasks in tasksByFlat)
        {
            var flatmates = _db.User
                .Where(u => _db.flatUser.Any(fu =>
                    fu.userid == u.Id &&
                    fu.flatID == flatTasks.Key &&
                    fu.IsActive))
                .ToList();

            if (!flatmates.Any())
            {
                continue;
            }

            foreach (var task in flatTasks)
            {
                if (task.LastRotationDate + task.Frequency <= currentTime)
                {
                    var currentUserIndex = flatmates.FindIndex(u => u.Id == task.CurrentUserId);

                    var nextUserIndex = (currentUserIndex + 1) % flatmates.Count;
                    task.CurrentUserId = flatmates[nextUserIndex].Id;
                    task.LastRotationDate = currentTime;
                }

                var nextRotationDate = task.LastRotationDate + task.Frequency;
                if (nextRotationDate < earliestNextRotation)
                {
                    earliestNextRotation = nextRotationDate;
                }
            }
        }

        _taskAssignmentManager.NextChangeDate = earliestNextRotation;
    }

}
