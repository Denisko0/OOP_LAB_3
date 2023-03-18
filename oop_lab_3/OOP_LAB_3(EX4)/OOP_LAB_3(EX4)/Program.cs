using System;
using System.Threading;
using System.Timers;
using System.Collections.Generic;

public class Workflow
{
    private List<Action> _actions = new List<Action>();

    public void AddAction(Action action)
    {
        _actions.Add(action);
    }

    public void Run()
    {
        foreach (var action in _actions)
        {
            action.Invoke();
        }
    }
}

public abstract class WorkflowAction
{
    public delegate void ActionCompletedEventHandler(object sender, EventArgs e);

    public event ActionCompletedEventHandler ActionCompleted;

    public abstract void Execute();

    protected virtual void OnActionCompleted()
    {
        ActionCompleted?.Invoke(this, EventArgs.Empty);
    }
}

public class CreateFileAction : WorkflowAction
{
    public delegate void FileCreatedEventHandler(object sender, EventArgs e);

    public event FileCreatedEventHandler FileCreated;

    public override void Execute()
    {

        OnActionCompleted();
        OnFileCreated();
    }

    protected virtual void OnFileCreated()
    {
        FileCreated?.Invoke(this, EventArgs.Empty);
    }
}

class WorkflowRunner
{
    private bool isRunning = false;

    public void RunWorkflow(Workflow workflow)
    {
        if (isRunning)
        {
            Console.WriteLine("Workflow is already running.");
            return;
        }

        isRunning = true;

        Console.WriteLine("Workflow started.");

        foreach (var action in workflow.Actions)
        {
            action.Execute();

            if (workflow.CanTransition(action))
            {
                workflow.Transition(action);
            }
            else
            {
                Console.WriteLine("Workflow finished.");
                break;
            }
        }

        isRunning = false;
    }
}

var workflow = new Workflow();
workflow.AddAction(new Action("Action 1", () => Console.WriteLine("Action 1 executed.")));
workflow.AddAction(new Action("Action 2", () => Console.WriteLine("Action 2 executed.")));
workflow.AddAction(new Action("Action 3", () => Console.WriteLine("Action 3 executed.")));

workflow.SetTransition("Action 1", "Action 2");
workflow.SetTransition("Action 2", "Action 3");

var runner = new WorkflowRunner();
runner.RunWorkflow(workflow);