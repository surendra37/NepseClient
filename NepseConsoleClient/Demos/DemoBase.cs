using System;
namespace NepseConsoleClient.Demos
{
    public class DemoBase
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public Action Command { get; set; }
        public DemoBase()
        {
        }
    }
}
