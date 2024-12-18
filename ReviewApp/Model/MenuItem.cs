using System;

namespace ReviewApp.Model
{
	public class MenuItem
	{
        public string Title { get; set; }
        public string Icon { get; set; }
        public Type TargetType { get; set; }
        public Command Command { get; internal set; }
    }
  
}

