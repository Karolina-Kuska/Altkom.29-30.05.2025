using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Models
{
    public abstract class Entity : INotifyPropertyChanged
    {
        public int Id { get; set; }

        //ShadowProperty
        //public bool IsDeleted { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
