﻿using TrackIt.Entities.Core;

namespace TrackIt.Entities;

public class User : Entity
{
  public string? Name { get; set; }
  
  public Email? Email { get; set; }
  
  public Password? Password { get; set; }

  public Hierarchy Hierarchy { get; set; } = Hierarchy.CLIENT;
  
  public double? Income { get; set; }

  public static User Create (Email email, Password password)
  {
    return new User
    {
      Email = email,
      Password = password
    };
  }

  public void Update (string name, double income)
  {
    Name = name;
    Income = income;
  }
}