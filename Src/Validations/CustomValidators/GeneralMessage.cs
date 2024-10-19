namespace RestApi.Src.Validations.CustomValidators
{
    public class GeneralMessage
    {
        public string NotEmptyMsg(string property) => $"{property} can not be empty";

        public string MinLenMsg(string property, int len) =>
            $"{property} can not be less than {len} characters";

        public string MaxLenMsg(string property, int len) =>
            $"{property} can not be more than {len} characters";
    }
}
