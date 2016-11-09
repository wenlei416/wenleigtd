using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GTD.DAL.Abstract;
using GTD.Models;

namespace GTD.DAL
{
    public class PomodoroRepository : GenericRepository<Pomodoro>, IPomodoroRepository
    {
    }
}