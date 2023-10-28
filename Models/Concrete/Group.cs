namespace UserAuthAPI.Models.Concrete
{
    public class Group : IEntity
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
    }
}
