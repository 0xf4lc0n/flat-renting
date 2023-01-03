﻿namespace FlatRenting.DTOs;

public class CreateCommentDto
{
    public required string Content { get; set; }
}

public class GetCommentDto
{
    public required string Content { get; set; }
    public required string UserName { get; set; }
}