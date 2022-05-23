using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Pokemon.Model;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System.IO;

namespace Pokemon.ViewModel
{
    class PokemonViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<Model.Pokemon> Lista { get; set; } = new ObservableCollection<Model.Pokemon>();


        private Model.Pokemon pokemon;

        public Model.Pokemon Pokemon
        {
            get { return pokemon; }
            set
            {
                pokemon = value;
                PropertyChanged(this, new PropertyChangedEventArgs(null));
            }
        }

        public string Vista { get; set; } = "Ver";


        public ICommand CancelarCommand { get; set; }
        public ICommand CambiarVistaCommand { get; set; }
        public ICommand AgregarCommand { get; set; }
        public ICommand GuardarCommand { get; set; }
        public ICommand EliminarCommand { get; set; }

        public PokemonViewModel()
        {
            CargarArchivo();
            CancelarCommand = new RelayCommand(Cancelar);
            CambiarVistaCommand = new RelayCommand<string>(CambiarVista);
            AgregarCommand = new RelayCommand(Agregar);
            GuardarCommand = new RelayCommand(Guardar);
            EliminarCommand = new RelayCommand(Eliminar);
        }

        private void Guardar()
        {
            if (Pokemon != null)
            {
                Lista[posicionPokemonEditar] = Pokemon;
                GuardarArchivo();
                Cancelar();
            }
        }

        private void Eliminar()
        {
            if (Pokemon != null)
            {
                Lista.Remove(Pokemon);
                GuardarArchivo();
                CambiarVista("Ver");
            }
        }

        private void Agregar()
        {
            if (Pokemon != null)
            {


                Lista.Add(Pokemon);

                CambiarVista("Ver");
                GuardarArchivo();

            }
        }

        private void GuardarArchivo()
        {
            var json = JsonConvert.SerializeObject(Lista);
            File.WriteAllText("Pokemons.json", json);
        }

        public void CargarArchivo()
        {
            if (File.Exists("Pokemons.json"))
            {
                var json = File.ReadAllText("Pokemons.json");
                var datos = JsonConvert
                    .DeserializeObject<ObservableCollection<Model.Pokemon>>(json);

                if (datos != null)
                {
                    Lista = datos;
                }
                else
                {
                    Lista = new ObservableCollection<Model.Pokemon>();
                }
            }
        }

        int posicionPokemonEditar;

        private void CambiarVista(string x)
        {
            Vista = x;


            if (Vista == "Agregar")
            {
                Pokemon = new Model.Pokemon();
            }

            if (Vista == "Editar")
            {
                if (Pokemon != null)
                {
                    Model.Pokemon clon = new Model.Pokemon()
                    {
                        Nombre = Pokemon.Nombre,
                        Tipo1 = Pokemon.Tipo1,
                        Tipo2 = Pokemon.Tipo2,
                        Categoria = Pokemon.Categoria,
                        Foto = Pokemon.Foto
                    };

                    posicionPokemonEditar = Lista.IndexOf(Pokemon);
                    Pokemon = clon;
                }
            }
            PropertyChange();
        }

        private void Cancelar()
        {
            CambiarVista("Ver");
            Pokemon = null;
        }

        void PropertyChange(string propiedad = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propiedad));
        }
    }
}
