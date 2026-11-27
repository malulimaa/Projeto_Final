namespace ControleTreinoApi.Exceptions;

public class ExercicioEmUsoException : Exception
{
    public ExercicioEmUsoException()
        : base("Este exercício está em um ou mais treinos. Remova-o dos treinos antes de excluir.") { }
}
