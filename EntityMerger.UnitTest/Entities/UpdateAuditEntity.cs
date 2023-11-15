﻿using System;

namespace EntityMerger.UnitTest.Entities
{
    public abstract class UpdateAuditEntity : CreateAuditEntity
    {
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
