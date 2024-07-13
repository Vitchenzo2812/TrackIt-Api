﻿using TrackIt.Infraestructure.Repository.Contracts;
using TrackIt.Infraestructure.Database.Contracts;
using TrackIt.Commands.Errors;
using TrackIt.Entities;
using MediatR;

namespace TrackIt.Commands.Auth.SignUp;

public class SignUpHandle : IRequestHandler<SignUpCommand, SignUpResponse>
{
  private readonly IUserRepository _userRepository;
 
  private readonly IUnitOfWork _unitOfWork;

  public SignUpHandle (
    IUserRepository userRepository, 
    IUnitOfWork unitOfWork
  )
  {
    _userRepository = userRepository;
    _unitOfWork = unitOfWork;
  }
  
  public async Task<SignUpResponse> Handle (SignUpCommand request, CancellationToken cancellationToken)
  {
    if (_userRepository.FindByEmail(Email.FromAddress(request.Payload.Email)) is not null)
      throw new EmailAlreadyInUseError();

    var user = User.Create(
      email: Email.FromAddress(request.Payload.Email),
      password: Password.Create(request.Payload.Password)
    );
    
    _userRepository.Save(user);

    await _unitOfWork.SaveChangesAsync();
    
    return new SignUpResponse(UserId: user.Id);
  }
}