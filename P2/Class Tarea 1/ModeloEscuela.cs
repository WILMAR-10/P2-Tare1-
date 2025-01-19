using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2.Class_Tarea_1
{
    public abstract class Persona
    {
        public string Nombre { get; set; }

        protected Persona(string nombre)
        {
            Nombre = nombre;
        }

        public abstract string InformacionCompleta { get; }
    }

    public class Estudiante : Persona
    {
        public int NumeroUnico { get; set; }

        public Estudiante(string nombre, int numeroUnico) : base(nombre)
        {
            NumeroUnico = numeroUnico;
        }

        public override string InformacionCompleta => $"{NumeroUnico} - {Nombre}";
    }

    public class Profesor : Persona
    {
        public List<Curso> Cursos { get; private set; } = new List<Curso>();

        public Profesor(string nombre) : base(nombre) { }

        public void AsignarCurso(Curso curso)
        {
            Cursos.Add(curso);
        }

        public override string InformacionCompleta => $"(Profesor) - {Nombre}";
    }

    public class Curso
    {
        public string Nombre { get; set; }
        public int RecuentoClases { get; set; }
        public int RecuentoEjercicios { get; set; }

        public Curso(string nombre, int recuentoClases, int recuentoEjercicios)
        {
            Nombre = nombre;
            RecuentoClases = recuentoClases;
            RecuentoEjercicios = recuentoEjercicios;
        }
    }

    public class Clase
    {
        public string Identificador { get; set; }
        public List<Estudiante> Estudiantes { get; private set; } = new List<Estudiante>();
        public List<Profesor> Profesores { get; private set; } = new List<Profesor>();

        public Clase(string identificador)
        {
            Identificador = identificador;
        }

        public void AgregarEstudiante(Estudiante estudiante)
        {
            Estudiantes.Add(estudiante);
        }

        public void AgregarProfesor(Profesor profesor)
        {
            Profesores.Add(profesor);
        }

        public string ObtenerResumen()
        {
            var resumen = new StringBuilder();
            resumen.AppendLine($"Clase: {Identificador}");
            resumen.AppendLine("Profesores:");
            Profesores.ForEach(p => resumen.AppendLine($"- {p.Nombre}"));

            resumen.AppendLine("Estudiantes:");
            Estudiantes.ForEach(e => resumen.AppendLine($"- {e.Nombre} (ID: {e.NumeroUnico})"));

            return resumen.ToString();
        }
    }

    public class ModeloEscuela
    {
        public List<Clase> Clases { get; private set; } = new List<Clase>();
        public List<Profesor> Profesores { get; private set; } = new List<Profesor>();
        public List<Estudiante> Estudiantes { get; private set; } = new List<Estudiante>();

        public void AgregarClase(Clase clase)
        {
            Clases.Add(clase);
        }

        public void AgregarProfesor(Profesor profesor)
        {
            Profesores.Add(profesor);
        }

        public void AgregarEstudiante(Estudiante estudiante)
        {
            Estudiantes.Add(estudiante);
        }

        public Profesor BuscarProfesorPorNombre(string nombre)
        {
            return Profesores.FirstOrDefault(p => p.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));
        }

        public Estudiante BuscarEstudiantePorNumeroUnico(int numeroUnico)
        {
            return Estudiantes.FirstOrDefault(e => e.NumeroUnico == numeroUnico);
        }

        public bool AsignarProfesorAClase(string nombreProfesor, string identificadorClase)
        {
            var profesor = BuscarProfesorPorNombre(nombreProfesor);
            var clase = Clases.FirstOrDefault(c => c.Identificador.Equals(identificadorClase, StringComparison.OrdinalIgnoreCase));

            if (profesor != null && clase != null)
            {
                clase.AgregarProfesor(profesor);
                return true;
            }

            return false;
        }

        public bool AsignarEstudianteAClase(int numeroUnico, string identificadorClase)
        {
            var estudiante = BuscarEstudiantePorNumeroUnico(numeroUnico);
            var clase = Clases.FirstOrDefault(c => c.Identificador.Equals(identificadorClase, StringComparison.OrdinalIgnoreCase));

            if (estudiante != null && clase != null)
            {
                clase.AgregarEstudiante(estudiante);
                return true;
            }

            return false;
        }
    }
}