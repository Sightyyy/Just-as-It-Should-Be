public static class RouteEvaluator
{
    public static RouteType GetRoute(SaveData data)
    {
        if (data == null)
        {
            return RouteType.Neutral;
        }

        if (data.HasStoryFlag("KILLED_ANY_NPC"))
            return RouteType.Genocide;

        if (data.HasStoryFlag("SPARED_ALL_BOSSES"))
            return RouteType.Pacifist;

        return RouteType.Neutral;
    }
}

public enum RouteType
{
    Neutral,
    Pacifist,
    Genocide
}

