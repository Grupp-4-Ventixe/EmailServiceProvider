﻿namespace WebApi.Models;

public class EmailSendRequest
{
    //lista med mottagare
    public List<string> Recipients { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string PlainText { get; set; } = null!;
    public string Html { get; set; } = null!;
  
}
