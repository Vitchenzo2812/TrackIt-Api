﻿using TrackIt.Entities.Repository;
using TrackIt.Commands.Errors;
using TrackIt.Entities.Errors;
using MediatR;

namespace TrackIt.Queries.GetActivity;

public class GetActivityRealmHandle : IPipelineBehavior<GetActivityQuery, GetActivityResult>
{
  private readonly IUserRepository _userRepository;
  private readonly IActivityRepository _activityRepository;
  private readonly IActivityGroupRepository _activityGroupRepository;

  public GetActivityRealmHandle (
    IUserRepository userRepository,
    IActivityRepository activityRepository,
    IActivityGroupRepository activityGroupRepository
  )
  {
    _userRepository = userRepository;
    _activityRepository = activityRepository;
    _activityGroupRepository = activityGroupRepository;
  }
  
  public async Task<GetActivityResult> Handle (GetActivityQuery request, RequestHandlerDelegate<GetActivityResult> next, CancellationToken cancellationToken)
  {
    if (request.Session is null)
      throw new ForbiddenError();

    var user = await _userRepository.FindById(request.Session.Id);

    if (user is null)
      throw new NotFoundError("User not found");

    if (!user.EmailValidated)
      throw new EmailMustBeValidatedError();
    
    if (await _activityGroupRepository.FindById(request.Params.ActivityGroupId) is null)
      throw new NotFoundError("Activity Group not found");
    
    if (await _activityRepository.FindById(request.Params.ActivityId) is null)
      throw new NotFoundError("Activity not found");
    
    return await next();
  }
}