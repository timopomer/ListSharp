

public static string getString(object thevar) {
if (thevar.GetType() == typeof(Int32)) return "[0]" + (Int32) thevar + "[/0]";
if (thevar.GetType() == typeof(string)) return "[0]" + (string) thevar + "[/0]";
if (thevar.GetType() == typeof(string[])) return arr2str((string[]) thevar);
return "Show Error";
}

public static string SHOW_F(object thein) {
return System.Environment.NewLine +
		     "---------------output--------------" +
		     System.Environment.NewLine +
		     getString(thein) +
		     System.Environment.NewLine + "-----------------------------------" +
		     System.Environment.NewLine;
}

public static void DISP_F(object ob)
{
if ((ob).GetType() == typeof(int))
ob = (int)ob + "";

Console.WriteLine(ob.GetType() == typeof(string) ? ((string)ob) : String.Join(Environment.NewLine, (string[])ob));
}





public static string arr2str(string[] arr)
{
return String.Join(Environment.NewLine,arr.Select((n,i)=>"[" + i + "]" + n + "[/" + i + "]"));
}



public static string[] EXTRACT_F(string[] arr, string delimiter, int collumnum)
{
collumnum--;
return arr.Select(n=> Regex.Split(n, delimiter).ElementAtOrDefault(collumnum)).ToArray();
}


public static string[] COMBINE_F(string[][] srar, string bywhat)
{
return Enumerable.Range(0,srar.Max(n=>n.Length)).Select(i=>string.Join(bywhat, srar.Select(m=>m.ElementAtOrDefault(i)))).ToArray();
}

public static void OUTP_F(string path, object thevar)
{
if (thevar.GetType() == typeof(string)) {
System.IO.File.WriteAllText(path, (string) thevar);
}
if (thevar.GetType() == typeof(string[])) {
System.IO.File.WriteAllLines(path, (string[]) thevar);
}
}
public static void OPEN_F(string path) {
System.Diagnostics.Process.Start(path);
}

public static string CHOOSEFILE_F(string title)
{
            
OpenFileDialog ofd = new OpenFileDialog();
ofd.Title = title;
Thread myth;
DialogResult result = DialogResult.None;
myth = new Thread(new System.Threading.ThreadStart(()=> {
result = ofd.ShowDialog();
while (result != DialogResult.OK)
result = ofd.ShowDialog();
}));
myth.SetApartmentState(ApartmentState.STA);
myth.Start();
while (result != DialogResult.OK)
Thread.Sleep(1);


return ofd.FileName;
}

public static string CHOOSEFOLDER_F(string title)
{

FolderBrowserDialog fbd = new FolderBrowserDialog();
fbd.Description = title;
Thread myth;
DialogResult result = DialogResult.None;
myth = new Thread(new System.Threading.ThreadStart(() => {
result = fbd.ShowDialog();
while (result != DialogResult.OK)
result = fbd.ShowDialog();
}));
myth.SetApartmentState(ApartmentState.STA);
myth.Start();
while (result != DialogResult.OK)
Thread.Sleep(1);


return fbd.SelectedPath;
}

public static object INPT_F(Type type)
{
	string inp = Console.ReadLine();
	if(type==typeof(int))
		return long.Parse(inp);

	return inp; //implicit string
}

public static string[] GETLINES_F(string[] ins, System.Collections.Generic.List<int> range)
{
return range.Select(n => inRange(ins.Length,n-1)?ins[n-1]:"").ToArray();
}

public static bool inRange(int length,int element)
{
return length>element && element >= 0;
}

public static IEnumerable<int> EdgeRange(double start_d,double end_d)
{
int start = (int)start_d;
int end = (int)end_d;

return start < end ? Enumerable.Range(start, end - start + 1) : Enumerable.Range(end, start - end + 1).Reverse();
}


public static string[] ROWSPLIT_F(object ob, string delimiter)
{
if (delimiter == "<newline>") delimiter = System.Environment.NewLine;
return ob.GetType() == typeof(string) ? Regex.Split(((string)ob), delimiter) : ((string[])ob).SelectMany(n => Regex.Split((n), delimiter)).ToArray();
}


public static object REPLACE_F(string toReplace, string replaceWith,object ob)
{
return ob.GetType() == typeof(string) ? ((string)ob).Replace(toReplace, replaceWith) : (object)((string[])ob).Select(n=>n.Replace(toReplace, replaceWith)).ToArray();
}

public static object ADD_F(Type type,params object[] toadd)
{
string[] temp = toadd.SelectMany(n => n.GetType() == typeof(string) ? new string[] { (string)n } : (string[])n).ToArray();
return type == typeof(string) ? (object)String.Join("", temp) : temp;
}



public static string DOWNLOAD_F(string url,int maxtries)
{

string r = "";
int count = 1;
WebClient wb = new WebClient();
    wb.Headers.Add("User-Agent: Other");
while (r == "")
{

try
{
r = wb.DownloadString(url);
}
catch
{
count++;
if (count >= maxtries)
{
DISP_F("failed downloading.. returning empty STRG");
return "";
}
}


}
return r;
}


public static object GETRANGE_F(object ob, int start, int end)
{
start = Math.Max(start-1,0);
string[] temp_rows = ob.GetType() == typeof(string) ? new string[] { (string)ob } : (string[])ob;
temp_rows = temp_rows.Select(n=>(start > n.Length-1 || end <= start)?"":n.Substring(start, Math.Min(end, n.Length) - start)).ToArray();

return ob.GetType() == typeof(string) ? (object)temp_rows[0] : temp_rows;
}

public static int returnLength(object inp)
{
if (inp.GetType() == typeof(int))
inp = ((int)inp).ToString();
return inp.GetType() == typeof(string) ? ((string)inp).Length : ((string[])inp).Length;
}


public static object GETBETWEEN_F(object ob, string start, string end)
{  
string[] temp_rows = ob.GetType() == typeof(string) ? new string[] { (string)ob } : (string[])ob;
temp_rows = temp_rows.Select(n => (string)GETRANGE_F(n,n.IndexOf(start)+1+start.Length,n.LastIndexOf(end)!=-1? n.LastIndexOf(end):n.Length)).ToArray();

return ob.GetType() == typeof(string) ? (object)temp_rows[0] : temp_rows;
}




