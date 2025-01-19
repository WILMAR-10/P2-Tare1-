using P2.Clases_Tarea_1;
using P2.Class_Tarea_1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace P2
{
    public partial class Form1 : Form
    {
        private ModeloEscuela modeloEscuela = new ModeloEscuela();
        private Shape shape;

        public Form1()
        {
            InitializeComponent();
            InicializarComboBoxFiguras();
            InicializarComboBoxProfesores();
            ActualizarComboBoxProfesores();
            ActualizarComboBoxClases();
        }

        private void InicializarComboBoxFiguras()
        {
            comboBoxSeleccionarFigura.Items.Clear();
            comboBoxSeleccionarFigura.Items.AddRange(new string[] { "Rectangle", "Triangle", "Circle" });
        }

        private void InicializarComboBoxProfesores()
        {
            comboBoxSeleccionarProfesor.Items.Clear();
            var profesores = modeloEscuela.Clases.SelectMany(c => c.Profesores).Distinct().ToList();

            if (profesores.Any())
            {
                comboBoxSeleccionarProfesor.Items.AddRange(profesores.ToArray());
                comboBoxSeleccionarProfesor.DisplayMember = "Nombre";
            }
            else
            {
                MessageBox.Show("No hay profesores disponibles. Por favor, agregue profesores antes de crear una clase.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ActualizarComboBoxProfesores()
        {
            comboBoxSeleccionarProfesor.Items.Clear();

            var profesores = modeloEscuela.Profesores;

            if (profesores.Any())
            {
                comboBoxSeleccionarProfesor.Items.AddRange(profesores.ToArray());
                comboBoxSeleccionarProfesor.DisplayMember = "Nombre";

            }
            else
            {
                comboBoxSeleccionarProfesor.Items.Add("No hay profesores disponibles");
                comboBoxSeleccionarProfesor.SelectedIndex = 0;
            }
        }


        #region Clases
        private void buttonAgregarClase_Click(object sender, EventArgs e)
        {
            string nombreClase = textBoxNombreClase.Text.Trim();

            if (string.IsNullOrEmpty(nombreClase))
            {
                MessageBox.Show("Por favor, ingrese un nombre para la clase.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!(comboBoxSeleccionarProfesor.SelectedItem is Profesor profesorSeleccionado))
            {
                MessageBox.Show("Por favor, seleccione un profesor para la clase.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var nuevaClase = new Clase(nombreClase);
            nuevaClase.AgregarProfesor(profesorSeleccionado);

            modeloEscuela.AgregarClase(nuevaClase);
            ActualizarListaClases();

            textBoxNombreClase.Clear();
            comboBoxSeleccionarProfesor.SelectedIndex = -1;
            ActualizarComboBoxClases();
            MessageBox.Show($"Clase '{nombreClase}' asignada al profesor {profesorSeleccionado.Nombre}.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ButtonEliminarClase_Click(object sender, EventArgs e)
        {
            if (listClases.SelectedItem is Clase claseSeleccionada)
            {
                modeloEscuela.Clases.Remove(claseSeleccionada);
                ActualizarListaClases();
                ActualizarComboBoxClases();
            }
            else
            {
                MessageBox.Show("Seleccione una clase para eliminar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void textBoxBuscarClases_TextChanged(object sender, EventArgs e)
        {
            string filtro = textBoxBuscarClases.Text.ToLower();
            var clasesFiltradas = modeloEscuela.Clases
                .Where(c => c.Identificador.ToLower().Contains(filtro))
                .ToList();

            listClases.DataSource = null;
            listClases.DataSource = clasesFiltradas;
            listClases.DisplayMember = "Identificador";
        }

        private void ActualizarListaClases()
        {
            listClases.DataSource = null;
            listClases.DataSource = modeloEscuela.Clases;
            listClases.DisplayMember = "Identificador";
        }

        private void buttonAgregarEstudiante_Click(object sender, EventArgs e)
        {
            string nombreEstudiante = textBoxNombre.Text.Trim();
            if (string.IsNullOrEmpty(nombreEstudiante))
            {
                MessageBox.Show("Por favor, ingrese un nombre válido para el estudiante.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(textBoxCodigoUnico.Text, out int codigo) || codigo <= 0)
            {
                MessageBox.Show("Por favor, ingrese un código único válido para el estudiante.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var nuevoEstudiante = new Estudiante(nombreEstudiante, codigo);

            if (modeloEscuela.Estudiantes.Any(est => est.NumeroUnico == codigo))
            {
                MessageBox.Show($"Ya existe un estudiante con el código {codigo}.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            modeloEscuela.AgregarEstudiante(nuevoEstudiante);

            if (listClases.SelectedItem is Clase claseSeleccionada)
            {
                claseSeleccionada.AgregarEstudiante(nuevoEstudiante);
                ActualizarListaPersonas(claseSeleccionada);
            }

            textBoxNombre.Clear();
            textBoxCodigoUnico.Clear();

            MessageBox.Show($"Estudiante '{nombreEstudiante}' (ID: {codigo}) agregado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonAgregarProfesor_Click(object sender, EventArgs e)
        {
            string nombreProfesor = textBoxNombre.Text.Trim();

            if (!string.IsNullOrEmpty(nombreProfesor))
            {
                var nuevoProfesor = new Profesor(nombreProfesor);

                modeloEscuela.AgregarProfesor(nuevoProfesor);

                if (listClases.SelectedItem is Clase claseSeleccionada)
                {
                    claseSeleccionada.AgregarProfesor(nuevoProfesor);
                    ActualizarListaPersonas(claseSeleccionada);
                }

                ActualizarComboBoxProfesores();
                textBoxNombre.Clear();

                MessageBox.Show($"Profesor '{nombreProfesor}' agregado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Por favor, ingrese un nombre válido para el profesor.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void buttonEliminarPorCodigo_Click(object sender, EventArgs e)
        {
            if (listPersonas.SelectedItem is Persona personaSeleccionada)
            {
                if (listClases.SelectedItem is Clase claseSeleccionada)
                {
                    if (personaSeleccionada is Estudiante estudianteSeleccionado)
                    {
                        claseSeleccionada.Estudiantes.Remove(estudianteSeleccionado);
                    }
                    else if (personaSeleccionada is Profesor profesorSeleccionado)
                    {
                        claseSeleccionada.Profesores.Remove(profesorSeleccionado);
                    }

                    ActualizarListaPersonas(claseSeleccionada);
                    MessageBox.Show($"'{personaSeleccionada.Nombre}' eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Seleccione una persona para eliminar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void textBoxBuscarPersonas_TextChanged(object sender, EventArgs e)
        {
            if (listClases.SelectedItem is Clase claseSeleccionada)
            {
                string filtro = textBoxBuscarPersonas.Text.ToLower();

                var personasFiltradas = claseSeleccionada.Estudiantes
                    .Where(est => est.InformacionCompleta.ToLower().Contains(filtro))
                    .Cast<Persona>()
                    .Union(claseSeleccionada.Profesores.Where(prof => prof.InformacionCompleta.ToLower().Contains(filtro)))
                    .ToList();

                listPersonas.DataSource = null;
                listPersonas.DataSource = personasFiltradas;
                listPersonas.DisplayMember = "InformacionCompleta";
            }
            else
            {
                MessageBox.Show("Seleccione una clase primero.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ActualizarListaPersonas(Clase clase)
        {
            var personas = clase.Estudiantes.Cast<Persona>()
                .Union(clase.Profesores)
                .ToList();

            listPersonas.DataSource = null;
            listPersonas.DataSource = personas;
            listPersonas.DisplayMember = "InformacionCompleta";
        }


        #endregion

        #region Escuela

        private void buttonAgregarEstudianteAClase_Click(object sender, EventArgs e)
        {
            if (!(comboBoxSeleccionarClase.SelectedItem is Clase claseSeleccionada))
            {
                MessageBox.Show("Por favor, seleccione una clase.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(textBoxCodigoEstudiante.Text, out int codigo) || codigo <= 0)
            {
                MessageBox.Show("Por favor, ingrese un código de estudiante válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var estudiante = modeloEscuela.Estudiantes.FirstOrDefault(est => est.NumeroUnico == codigo);

            if (estudiante != null)
            {
                if (!claseSeleccionada.Estudiantes.Contains(estudiante))
                {
                    claseSeleccionada.AgregarEstudiante(estudiante);
                    ActualizarListaEstudiantesDeClase(claseSeleccionada);
                    MessageBox.Show($"Estudiante '{estudiante.Nombre}' agregado a la clase '{claseSeleccionada.Identificador}'.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"El estudiante '{estudiante.Nombre}' ya está en la clase.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Estudiante no encontrado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void buttonEliminarEstudianteDeClase_Click(object sender, EventArgs e)
        {
            if (!(listEstudiantesDeLaClase.SelectedItem is Estudiante estudianteSeleccionado))
            {
                MessageBox.Show("Por favor, seleccione un estudiante para eliminar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!(comboBoxSeleccionarClase.SelectedItem is Clase claseSeleccionada))
            {
                MessageBox.Show("Por favor, seleccione una clase.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            claseSeleccionada.Estudiantes.Remove(estudianteSeleccionado);
            ActualizarListaEstudiantesDeClase(claseSeleccionada);

            MessageBox.Show($"Estudiante '{estudianteSeleccionado.Nombre}' eliminado de la clase '{claseSeleccionada.Identificador}'.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void textBoxBuscarEstudiantesDeLaClase_TextChanged(object sender, EventArgs e)
        {
            if (comboBoxSeleccionarClase.SelectedItem is Clase claseSeleccionada)
            {
                string filtro = textBoxBuscarEstudiantesDeLaClase.Text.ToLower();

                var estudiantesFiltrados = claseSeleccionada.Estudiantes
                    .Where(est => est.Nombre.ToLower().Contains(filtro))
                    .ToList();

                listEstudiantesDeLaClase.DataSource = null;
                listEstudiantesDeLaClase.DataSource = estudiantesFiltrados;
                listEstudiantesDeLaClase.DisplayMember = "Nombre";
            }
            else
            {
                MessageBox.Show("Por favor, seleccione una clase primero.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void comboBoxSeleccionarClase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxSeleccionarClase.SelectedItem is Clase claseSeleccionada)
            {
                ActualizarListaEstudiantesDeClase(claseSeleccionada);
            }
        }

        private void ActualizarComboBoxClases()
        {
            comboBoxSeleccionarClase.Items.Clear();

            if (modeloEscuela.Clases.Any())
            {
                comboBoxSeleccionarClase.Items.AddRange(modeloEscuela.Clases.ToArray());
                comboBoxSeleccionarClase.DisplayMember = "Identificador";
            }
            else
            {
                comboBoxSeleccionarClase.Items.Add("No hay clases disponibles");
                comboBoxSeleccionarClase.SelectedIndex = 0;
            }
        }

        private void ActualizarListaEstudiantesDeClase(Clase clase)
        {
            listEstudiantesDeLaClase.DataSource = null;
            listEstudiantesDeLaClase.DataSource = clase.Estudiantes;
            listEstudiantesDeLaClase.DisplayMember = "Nombre";
        }
        #endregion

        #region Figuras Geométricas
        private void buttonCalcularArea_Click_1(object sender, EventArgs e)
        {
            if (comboBoxSeleccionarFigura.SelectedItem == null)
            {
                MessageBox.Show("Por favor, selecciona una figura.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string figuraSeleccionada = comboBoxSeleccionarFigura.SelectedItem.ToString();
            double dimension1 = 0, dimension2 = 0;

            if (!double.TryParse(textBoxDimension1.Text, out dimension1) || dimension1 <= 0)
            {
                MessageBox.Show("Por favor, ingresa un valor válido para la primera dimensión.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (figuraSeleccionada != "Circle")
            {
                if (!double.TryParse(textBoxDimension2.Text, out dimension2) || dimension2 <= 0)
                {
                    MessageBox.Show("Por favor, ingresa un valor válido para la segunda dimensión.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            Shape figura;
            switch (figuraSeleccionada)
            {
                case "Rectangle":
                    figura = new Rectangulo(dimension1, dimension2);
                    break;
                case "Triangle":
                    figura = new Triangulo(dimension1, dimension2);
                    break;
                case "Circle":
                    figura = new Circulo(dimension1);
                    break;
                default:
                    MessageBox.Show("Figura no reconocida.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
            }

            double area = figura.CalcularArea();
            labelResultado.Text = $"El área del {figuraSeleccionada} es: {area:F2}";
        }

        private void comboBoxSeleccionarFigura_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxSeleccionarFigura.SelectedItem == null)
            {
                return;
            }

            string shapeSelected = comboBoxSeleccionarFigura.SelectedItem.ToString();

            switch (shapeSelected)
            {
                case "Rectangle":
                    pictureBoxFigura.Image = Properties.Resources.RectangleImage;
                    textBoxDimension1.Enabled = true;
                    textBoxDimension2.Enabled = true;
                    break;
                case "Triangle":
                    pictureBoxFigura.Image = Properties.Resources.TriangleImage;
                    textBoxDimension1.Enabled = true;
                    textBoxDimension2.Enabled = true;
                    break;
                case "Circle":
                    pictureBoxFigura.Image = Properties.Resources.CircleImage;
                    textBoxDimension1.Enabled = true;
                    textBoxDimension2.Enabled = false;
                    textBoxDimension2.Text = "";
                    break;
                default:
                    pictureBoxFigura.Image = Properties.Resources.estrellaC;
                    textBoxDimension1.Enabled = false;
                    textBoxDimension2.Enabled = false;
                    break;
            }

            labelResultado.Text = "";
        }

        private void buttonCalcularArea_Click(object sender, EventArgs e)
        {
            if (comboBoxSeleccionarFigura.SelectedItem == null)
            {
                MessageBox.Show("Please select a shape.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string shapeSelected = comboBoxSeleccionarFigura.SelectedItem.ToString();
            double dimension1 = 0, dimension2 = 0;

            if (!double.TryParse(textBoxDimension1.Text, out dimension1) || dimension1 <= 0)
            {
                MessageBox.Show("Please enter a valid value for the first dimension.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (shapeSelected != "Circle")
            {
                if (!double.TryParse(textBoxDimension2.Text, out dimension2) || dimension2 <= 0)
                {
                    MessageBox.Show("Please enter a valid value for the second dimension.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            switch (shapeSelected)
            {
                case "Rectangle":
                    shape = new Rectangulo(dimension1, dimension2);
                    break;
                case "Triangle":
                    shape = new Triangulo(dimension1, dimension2);
                    break;
                case "Circle":
                    shape = new Circulo(dimension1);
                    break;
                default:
                    MessageBox.Show("Shape not recognized.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
            }

            double area = shape.CalcularArea();
            labelResultado.Text = $"The area of the {shapeSelected} is: {area:F2}";
        }

        private void buttonLimpiar_Click(object sender, EventArgs e)
        {
            comboBoxSeleccionarFigura.SelectedIndex = -1;
            textBoxDimension1.Text = "";
            textBoxDimension2.Text = "";
            textBoxDimension1.Enabled = false;
            textBoxDimension2.Enabled = false;
            pictureBoxFigura.Image = Properties.Resources.estrellaC;
            labelResultado.Text = "";
        }
        #endregion
    }
}
