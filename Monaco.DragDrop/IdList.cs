namespace Monaco.DragDrop;
public class IdList : List<string>
{
    public static IdList Parse(string input)
    {
        var ids = input.Split(',')
            .Select(x => x.Trim());

        var list = new IdList();
        list.AddRange(ids);
        return list;
    }
}
