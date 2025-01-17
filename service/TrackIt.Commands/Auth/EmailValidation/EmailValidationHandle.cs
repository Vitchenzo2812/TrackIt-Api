﻿using Session = TrackIt.Infraestructure.Security.Models.Session;
using TrackIt.Infraestructure.Security.Contracts;
using TrackIt.Infraestructure.Database.Contracts;
using TrackIt.Entities.Errors;
using TrackIt.Entities.Core;
using MediatR;
using TrackIt.Entities.Repository;

namespace TrackIt.Commands.Auth.EmailValidation;

public class EmailValidationHandle : IRequestHandler<EmailValidationCommand, Session>
{
  private readonly IUserRepository _userRepository;

  private readonly ITicketRepository _ticketRepository;

  private readonly ISessionService _sessionService;

  private readonly IUnitOfWork _unitOfWork;

  public EmailValidationHandle (
    IUserRepository userRepository,
    ITicketRepository ticketRepository,
    ISessionService sessionService,
    IUnitOfWork unitOfWork
  )
  {
    _userRepository = userRepository;
    _ticketRepository = ticketRepository;
    _sessionService = sessionService;
    _unitOfWork = unitOfWork;
  }
  
  public async Task<Session> Handle (EmailValidationCommand request, CancellationToken cancellationToken)
  {
    var user = await _userRepository.FindById(request.Payload.UserId);
    
    if (user is null)
      throw new NotFoundError("User not found");
    
    var ticket = await _ticketRepository.FindLastWithTypeAndSituation(
      request.Payload.UserId, TicketType.EMAIL_VERIFICATION, TicketSituation.OPEN
    );

    if (ticket is null)
      throw new NotFoundError("Ticket not found");
    
    ticket.Close(request.Payload.Code);
    
    user.ValidateEmail();
    
    await _unitOfWork.SaveChangesAsync();

    return await _sessionService.Create(user.Id);
  }
}