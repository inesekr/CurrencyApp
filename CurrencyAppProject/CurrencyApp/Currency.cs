namespace CurrencyApp
{
    public class Currency
    {
        public string Code { get; set; }  
        public string Name { get; set; }  
        public string Symbol { get; set; } 

        public Currency(string code, string name, string symbol)
        {
            Code = code;
            Name = name;
            Symbol = symbol;
        }
    }
}
