using System;

namespace Helper.ServiceGateways.Models;

public interface IAuditable
{
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }
}