public static class RouteEvaluator
{
    public static RouteType GetRoute(SaveData data)
    {
        if (data.storyFlags.Contains("KILLED_ANY_NPC"))
            return RouteType.Genocide;

        if (data.storyFlags.Contains("SPARED_ALL_BOSSES"))
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

