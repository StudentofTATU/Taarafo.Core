﻿// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

using System;

namespace Taarafo.Core.Models.Posts.Exceptions
{
    public class PostServiceException : Exception
    {
        public PostServiceException(Exception message)
            : base("Post service error occurred, contact support.", message) { }
    }
}
