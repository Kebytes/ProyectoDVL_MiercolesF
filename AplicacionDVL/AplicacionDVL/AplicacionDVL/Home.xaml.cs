﻿using AplicacionDVL.API;
using AplicacionDVL.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace AplicacionDVL
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Home : ContentPage
	{
        public Home ()
		{
			InitializeComponent ();
            detallesPedido.ItemTapped += DetallesPedido_ItemTapped;
            this.BindingContext = this;
        }

        private void DetallesPedido_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null) return;

            if (sender is ListView lv) lv.SelectedItem = null;
        }

        protected async override void OnAppearing()
        {
            int x = Navigation.NavigationStack.IndexOf(this) - 1;
            if (x >= 0)
            {
                var previousPage = Navigation.NavigationStack[Navigation.NavigationStack.IndexOf(this) - 1];
                Navigation.RemovePage(previousPage);
            }

            base.OnAppearing();

            if (Application.Current.Properties.ContainsKey("Usuario"))
            {
                Clientes cliente = JsonConvert.DeserializeObject<Clientes>(Application.Current.Properties["Usuario"].ToString());
                string pedidos = await Pedidos_Controller.GetPedidosOnly(cliente.id_Clientes);
                Application.Current.Properties["Pedidos"] = pedidos;
                await Application.Current.SavePropertiesAsync();
            }

            ListaElementos elementos = new ListaElementos();
            detallesPedido.ItemsSource = elementos.pedidosAgendados;
        }

        public class ListaElementos
        {
            public List<Models.Pedido> elementos { get; set; }
            public List<Models.Pedido> pedidosAgendados { get; set; }

            public ListaElementos()
            {
                elementos = new List<Models.Pedido>();
                pedidosAgendados = new List<Pedido>();
                loadElementos();
            }

            public void loadElementos()
            {
                if (Application.Current.Properties.ContainsKey("Pedidos"))
                {
                    elementos = JsonConvert.DeserializeObject<List<Models.Pedido>>(Application.Current.Properties["Pedidos"].ToString());

                    for (int i = 0; i < elementos.Count; i++)
                    {
                        if (elementos[i].Estatus.Equals("A"))
                        {
                            elementos[i].OracionFecha = elementos[i].Fecha_Programada.ToString("dd/MMMM/yyyy", CultureInfo.CreateSpecificCulture("es-ES")).Replace("/", " ");

                            //if (elementos[i].Litros_Magna > 0)
                            //{
                                elementos[i].OracionMagna = elementos[i].Litros_Magna.ToString("#,##0.###") + " L";
                                elementos[i].totalLitros += elementos[i].Litros_Magna;
                            //}

                            //if (elementos[i].Litros_Premium > 0)
                            //{
                                elementos[i].OracionPremium = elementos[i].Litros_Premium.ToString("#,##0.###") + " L";
                                elementos[i].totalLitros += elementos[i].Litros_Premium;
                            //}

                            //if (elementos[i].Litros_Diesel > 0)
                            //{
                                elementos[i].OracionDiesel = elementos[i].Litros_Diesel.ToString("#,##0.###") + " L";
                                elementos[i].totalLitros += elementos[i].Litros_Diesel;
                            //}

                            elementos[i].TotalLitros = elementos[i].totalLitros.ToString("#,##0.###") + " L";

                            //if (elementos[i].Estatus.Equals("A"))
                            //{
                                elementos[i].OracionEstatus = "Agendado";
                                elementos[i].OracionImagen = "confirmado.png";

                            pedidosAgendados.Add(elementos[i]);
                            //}
                        }
                    }
                }
            }
        }
    }
}