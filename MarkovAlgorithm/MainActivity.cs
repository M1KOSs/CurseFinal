using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

interface rule
{

    void replace(string substring, string changing);
    void Delete(String substr);
    void ReadRule(string rule1, string rule2, int count);
    void ReadOriginal(string str);
    bool Contains(string str);
    string Result();
    bool end(int i);
    void CycleControl();
    void Clear();


}




class MarkovAlg : rule
{
    public MarkovAlg()
    {

    }

    public string[,] rules = new string[100, 2];
    string originalString = "";


    public void replace(string substring, string changing)
    {
        int i = this.originalString.IndexOf(substring);
        this.originalString = this.originalString.Remove(i, substring.Length).Insert(i, changing);
    }

    public void Delete(String substr)
    {
        this.originalString.Replace(substr, "");
    }

    public void ReadRule(string rule1, string rule2, int count)
    {
        this.rules[count, 0] = rule1;
        this.rules[count, 1] = rule2;

    }

    public void ReadOriginal(string str)
    {
        this.originalString = str;
    }

    public bool Contains(string str)
    {
        if (this.originalString.Contains(str))
            return true;
        else return false;
    }

    public string Result()
    {
        return this.originalString;
    }

    public bool end(int i)
    {
        if (this.rules[i, 1].Contains("."))
            return true;
        else return false;
    }


    public void CycleControl()
    {
        this.originalString = " To many steps. Maybe eror in  incomer rule.";
    }

    public void Clear()
    {
        this.originalString = "";
        Array.Clear(rules, 0, rules.Length);
    }
}




namespace MarkovAlgorithm
{
    [Activity(Label = "MarkovAlgorithm", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 0;
        int i = 0;
        int k = 0;




        Spinner spinnerRule, spinnerStep;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            string[] Example = new string[0];
            string[] Example1 = new string[0];
            string[] ClearArray = { "" };




            spinnerRule = FindViewById<Spinner>(Resource.Id.spinner1);
            spinnerStep = FindViewById<Spinner>(Resource.Id.spinner2);




            MarkovAlg Markov = new MarkovAlg();
            var RESULT = FindViewById<EditText>(Resource.Id.RESULT);
            // Get our button from the layout resource,
            // and attach an event to it
            Button nextRule = FindViewById<Button>(Resource.Id.nextRule);
            Button ansver = FindViewById<Button>(Resource.Id.ansver);
            Button ACCEPT = FindViewById<Button>(Resource.Id.ACCEPT);
            Button Exit = FindViewById<Button>(Resource.Id.Exit);
            Button Help = FindViewById<Button>(Resource.Id.Help);
            Button Clear = FindViewById<Button>(Resource.Id.Clear);

            Clear.Click += delegate
            {
                Markov.Clear();
                Array.Clear(Example, 0, Example.Length);
                Array.Clear(Example1, 0, Example1.Length);
                count = 0;
                i = 0;
                k = 0;
                RESULT.Text = " All Clear";
                FindViewById<EditText>(Resource.Id.rule1).Text = "";
                FindViewById<EditText>(Resource.Id.rule2).Text = "";
                FindViewById<EditText>(Resource.Id.original).Text = "";
                spinnerRule.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, ClearArray);
                spinnerStep.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, ClearArray);









            };

            Exit.Click += delegate
            {
                this.FinishAffinity();
            };

            Help.Click += delegate
            {
                Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
                Android.App.AlertDialog alertDialog = builder.Create();
                alertDialog.SetTitle("Help");
                alertDialog.SetMessage("Empty symbol it's  '  "+ "  ' (Spacebar)  \n For enteryng Original word press Accept \n For eneryng every rule press Next Rule");


                alertDialog.Show();
            };

            ACCEPT.Click += delegate
            {
                Markov.ReadOriginal(FindViewById<EditText>(Resource.Id.original).Text);
            };

            nextRule.Click += delegate
            {
                if (FindViewById<EditText>(Resource.Id.rule1).Text == "" && FindViewById<EditText>(Resource.Id.rule2).Text == "")
                {
                    Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
                    Android.App.AlertDialog Validation = builder.Create();
                    Validation.SetTitle("Error");
                    Validation.SetMessage("One from rules must be diferent from NULL");
                    Validation.Show();
                }
                else
                {                  
                    Markov.ReadRule(FindViewById<EditText>(Resource.Id.rule1).Text, FindViewById<EditText>(Resource.Id.rule2).Text, count);
                    Array.Resize<string>(ref Example, count + 1);
                    Example[count] = (count + 1).ToString() + " rule - " + FindViewById<EditText>(Resource.Id.rule1).Text + "  -> " + " " + FindViewById<EditText>(Resource.Id.rule2).Text;
                    spinnerRule.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, Example);
                    count = count + 1;
                    FindViewById<EditText>(Resource.Id.rule1).Text = "";
                    FindViewById<EditText>(Resource.Id.rule2).Text = "";
                }
            };
            i = 0;
            ansver.Click += delegate
            {
                if (Markov.Result() == "")
                {
                    Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
                    Android.App.AlertDialog Error = builder.Create();
                    Error.SetTitle("Help");
                    Error.SetMessage("Enter original word");


                    Error.Show();
                }
                else
                { 



                    k = count;

                int RuleCount = 0;
                Array.Resize<string>(ref Example1, RuleCount + 1);
                Example1[RuleCount] = "Original String " + Markov.Result();
                spinnerStep.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, Example1);
                RuleCount = 1;

                int CycleControl = 0;
                {
                    while (i < count)
                    {
                        CycleControl++;
                        if (CycleControl > 1000)
                        {
                            Markov.CycleControl();
                            break;
                        }

                        if (Markov.Contains(Markov.rules[i, 0]))
                        {

                            Markov.replace(Markov.rules[i, 0], Markov.rules[i, 1]);                           
                            Array.Resize<string>(ref Example1, RuleCount + 1);
                            Example1[RuleCount] = (RuleCount + 1).ToString() + " step - " + Markov.Result();
                            spinnerStep.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, Example1);
                            RuleCount++;
                            if (Markov.end(i)) break;

                            i = 0;
                        }
                        else
                        {
                            i++;
                        }
                    };

                }
            }
                RESULT.Text = Markov.Result();






            };
        }
    }
}
