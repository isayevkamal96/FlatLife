using System;
using FlatLife.Database.ApplicationDbContext;
using FlatLife.Database.Entities;
using FlatLife.Mapping;
using Microsoft.EntityFrameworkCore;

namespace FlatLife.Models.FlatDTO;

public class FlatTaskResponseBodyMapper : BaseMapper<FlatTask, FlatTaskResponseBody>
{

    private readonly ApplicationDbContext _db;
    private readonly TaskAssignmentManager _taskAssignmentManager;

    public FlatTaskResponseBodyMapper(ApplicationDbContext db, TaskAssignmentManager taskAssignmentManager)
    {
        _db = db;
        _taskAssignmentManager = taskAssignmentManager;
    }
    public override FlatTaskResponseBody Map(FlatTask task)
    {
        return new FlatTaskResponseBody
        {
            Id = task.Id,
            TaskName = task.TaskName,
            Frequency = task.Frequency,
            Deadline = task.Deadline,
            AssignedToUserName = task.CurrentUser.Username,
        };
    }
    public async Task<FlatTask> Map(FlatTaskRequestBody flatTaskRequestBody, int activeFlatId)
    {
        var flatmates = _db.User.Where(u =>
         _db.flatUser.Any(fu => fu.userid == u.Id && fu.flatID == activeFlatId && fu.IsActive))
         .ToList();

        var firstTask = await _db.FlatTask
                   .Include(t => t.CurrentUser)
                   .Where(t => t.FlatId == activeFlatId)
                   .FirstOrDefaultAsync();



        TimeSpan? frequency = firstTask?.Frequency;
        TimeSpan defaultFrequency = TimeSpan.FromDays(7);

        return new FlatTask
        {
            TaskName = flatTaskRequestBody.TaskName,
            Frequency = flatTaskRequestBody.Frequency ?? frequency ?? defaultFrequency,
            CurrentUserId = flatmates[_taskAssignmentManager.CurrentResidentIndex].Id,
            FlatId = activeFlatId,
            LastRotationDate = DateTime.UtcNow
        };
    }
}
