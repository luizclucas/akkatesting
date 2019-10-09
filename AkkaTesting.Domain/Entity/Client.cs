using AkkaTesting.Domain.Interfaces;
using System;

namespace AkkaTesting.Domain.Entity
{
    public class Client : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string CPF { get; set; }
        public string City { get; set; }
    }
}
