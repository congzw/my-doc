namespace NbApp.Web.Models
{
    public class SystemdReadmeVo
    {
        public string srv_name { get; set; }
        public string srv_file_name { get; set; }
        public int srv_port { get; set; } = 8888;
        public string srv_file_source { get; set; }
        public string srv_file_target { get; set; }

        public static SystemdReadmeVo Create(string srv_name)
        {
            ///.config/systemd/user/
            var item = new SystemdReadmeVo();
            item.srv_name = srv_name;
            item.srv_file_name = $"{srv_name}.service";
            item.srv_file_source = $"./{item.srv_file_name}";
            item.srv_file_target = $"/home/a/.config/systemd/user/{item.srv_file_name}";
            return item;
        }
    }
}