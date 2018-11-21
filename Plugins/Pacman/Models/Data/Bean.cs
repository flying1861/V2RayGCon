namespace Pacman.Models.Data
{
    public class Bean
    {
        public string uid { get; set; }
        public string title { get; set; }
        public double index { get; set; }
        public bool isSelected { get; set; }

        public Bean()
        {
            uid = null;
            title = string.Empty;
            index = double.MaxValue;
            isSelected = true;
        }

        public Bean Clone()
        {
            return new Bean {
                uid=uid,
                title=title,
                index=index,
                isSelected=isSelected,
            };
        }
    }
}
