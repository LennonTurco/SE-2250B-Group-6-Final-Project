namespace DefaultNamespace;

public class ProgressionSystem
{
    private int progression;
    private List<string> inventory;

    public ProgressionSystem()
    {
        progression = 0;
        inventory = new List<string>();
    }

    public void ApplyUpgrade(string stat, int amount)
    {
        if (stat == "progression")
        {
            progression +=  amount;
            Debug.Log("Progression increased by " + amount + ". Total: " + progression);
        }
        else
        {
            Debug.Log("Unknown stat: " + stat);
        }
    }
}