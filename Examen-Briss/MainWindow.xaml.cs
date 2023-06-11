using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace ExamenBriss
{
    public partial class MainWindow : Window
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private DataTable estudiantesTable;
        private DataRowView selectedRow;

        public MainWindow()
        {
            InitializeComponent();
            LoadEstudiantes();
        }

        private SqlConnection CrearConexion()
        {
            return new SqlConnection(connectionString);
        }

        private void LoadEstudiantes()
        {
            try
            {
                using (SqlConnection connection = CrearConexion())
                {
                    string query = "SELECT Carnet, Nombre, Telefono, Grado FROM Estudiantes";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);

                    estudiantesTable = new DataTable();
                    adapter.Fill(estudiantesTable);

                    EstudiantesDataGrid.ItemsSource = estudiantesTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los estudiantes: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EstudiantesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EstudiantesDataGrid.SelectedItem != null)
            {
                selectedRow = (DataRowView)EstudiantesDataGrid.SelectedItem;

                TxtCarnetValue.Text = selectedRow["Carnet"].ToString();
                TxtNombre.Text = selectedRow["Nombre"].ToString();
                TxtTelefono.Text = selectedRow["Telefono"].ToString();
                TxtGrado.Text = selectedRow["Grado"].ToString();
            }
        }

        private void BtnActualizar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedRow != null)
                {
                    using (SqlConnection connection = CrearConexion())
                    {
                        string query = "UPDATE Estudiantes SET Nombre = @nombre, Telefono = @telefono, Grado = @grado WHERE Carnet = @carnet";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@nombre", TxtNombre.Text);
                        command.Parameters.AddWithValue("@telefono", TxtTelefono.Text);
                        command.Parameters.AddWithValue("@grado", TxtGrado.Text);

                        string carnet = selectedRow["Carnet"].ToString();
                        command.Parameters.AddWithValue("@carnet", carnet);

                        connection.Open();
                        command.ExecuteNonQuery();

                        LoadEstudiantes();
                        ClearInputs();
                    }
                }
                else
                {
                    MessageBox.Show("No se ha seleccionado ningún estudiante.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar los datos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedRow != null)
                {
                    MessageBoxResult result = MessageBox.Show("¿Está seguro de que desea eliminar este estudiante?", "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        using (SqlConnection connection = CrearConexion())
                        {
                            string query = "DELETE FROM Estudiantes WHERE Carnet = @carnet";
                            SqlCommand command = new SqlCommand(query, connection);
                            string carnet = selectedRow["Carnet"].ToString();
                            command.Parameters.AddWithValue("@carnet", carnet);

                            connection.Open();
                            command.ExecuteNonQuery();

                            LoadEstudiantes();
                            ClearInputs();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No se ha seleccionado ningún estudiante.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar el estudiante: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection connection = CrearConexion())
                {
                    string query = "INSERT INTO Estudiantes (Carnet, Nombre, Telefono, Grado) VALUES (@carnet, @nombre, @telefono, @grado)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@carnet", TxtCarnetValue.Text);
                    command.Parameters.AddWithValue("@nombre", TxtNombre.Text);
                    command.Parameters.AddWithValue("@telefono", TxtTelefono.Text);
                    command.Parameters.AddWithValue("@grado", TxtGrado.Text);

                    connection.Open();
                    command.ExecuteNonQuery();

                    LoadEstudiantes();
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar el estudiante: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearInputs()
        {
            TxtCarnetValue.Text = "";
            TxtNombre.Text = "";
            TxtTelefono.Text = "";
            TxtGrado.Text = "";
            selectedRow = null;
        }
    }
}