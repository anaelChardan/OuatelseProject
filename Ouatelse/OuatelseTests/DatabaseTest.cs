﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ouatelse;

namespace OuatelseTests
{
    [TestClass]
    public class DatabaseTest
    {
        [TestMethod]
        public void DatabaseConnection()
        {
            Database db = Database.Instance;
        }
    }
}