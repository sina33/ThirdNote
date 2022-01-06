﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ThirdNote.Models;

namespace ThirdNote.ViewModels
{
    public class Dashboard
    {
        public IEnumerable<Note> Pinned { get; set; }
        public IEnumerable<Note> Sorted { get; set; }
        public List<int> Strikes { get; set; }
    }
}