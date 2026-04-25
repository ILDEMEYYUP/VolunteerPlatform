using System;

namespace VolunteerPlatform.Domain.Entities;

// seeding e ihtiyaç duyulabilir
// defaulr yetenekleirn olması daha derli toplu olacak 
// bir list hazırlanığ seed yapacağım bir ara
public class Skill
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // exmp : 
    /*
        Skill{
            Id:"mwkmvowmnvoı2v023094u2d3" ,
            Name:"React",
            Description:"a framework to work on frontend"
        }*/
}