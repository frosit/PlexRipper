﻿namespace PlexRipper.Domain.Entities
{
    public class PlexAccountServer
    {
        public int PlexAccountId { get; set; }
        public virtual PlexAccount PlexAccount { get; set; }
        public int PlexServerId { get; set; }
        public virtual PlexServer PlexServer { get; set; }
    }
}
