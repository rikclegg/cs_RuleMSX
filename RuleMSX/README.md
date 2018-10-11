# RuleMSX - C#

C# implementation of the RuleMSX library. RuleMSX provides the core functionality of a rule engine. 

Rather than making use of a DSL to define the rules, it exposes a series of abstract classes. It facilitates the creation of complex if-this-then-that behavior.

## Getting Started

Once the library has been referenced in your project, create an instance of RuleMSX:-

	RuleMSX rmsx = new RuleMSX();

RuleMSX is divided into Rules, DataPoints and Actions.

Rules are organised into RuleSets:-

	RuleSet myRuleSet = this.rmsx.CreateRuleSet("MyRuleSet");

A RuleSet contains one or more Rules, and each Rule is made up of one or more RuleConditions. Each RuleCondition has a RuleEvaluator which is the code written by the developer. Each rule also has one or more RuleActions associated with it. When all the RuleConditions are met, the RuleAction is executed.

To create a Rule:-

	Rule myNewRule = myRuleSet.AddRule("NewRule");

To create a RuleCondition:-

	RuleCondition myRuleCondition = new RuleCondition("MyCondition", new MyConditionCode());

The 'MyConditionCode' class extends the RuleEvaluator abstract class, guarenteeing the presence of an Evaluate() method. This method must return a boolean value.

For example:-

	class MyConditionCode : RuleEvaluator
    {
        public MyConditionCode()
        {
            // constructor code
        }

        public override bool Evaluate(DataSet dataSet)
        {
			if(<some test>) {
				return True;
			}
			else
			{
				return False;
			}
        }
    }

Note that when the Evaluate() method is called, it is passed a DataSet to operate on.

Add the RuleCondition to the Rule:-

	myNewRule.AddRuleCondition(myRuleCondition);

Alternatively:-

	myRuleCondition.AddRuleCondition(new RuleCondition("MyCondition", new MyConditionCode());

When the RuleEvaluator of each of the RuleConditions associated with a Rule return True, then any Actions associated with the Rule will be fired. 

Actions are created independantly of a Rule, so that a single action can be reused across multiple Rules. An action consists of a Rule object, and an associated RuleEvaluator which is a extended by the developer.

The create an Action:-

	Action myAction = rmsx.CreateAction("MyAction", new MyActionCode());

The 'MyActionCode' class extends the ActionExecutor abstract class, guarenteeing the presence of an Execute() method. 

For example:-

    class MyActionCode: ActionExecutor
    {

        public MyActionCode()
        {
			// constructor code
        }

        public void Execute(DataSet dataSet)
        {
            // do something here
        }
    }

Note that when the Execute() method of the ActionExecutor is called, it is passed a DataSet to operate on.

Add the Action to the Rule:-

	myNewRule.AddAction(myAction);

Alternatively:-

	myNewRule.AddAction(rmsx.CreateAction("MyAction", new MyActionCode());


The data to be processed in a RuleSet is defined as DataPoints, which are organised into DataSets.

A DataPoint is a single named item of data that has an associated DataPointSource. The DataPointSource is an abstract class that the developer extends, which guarentees the presence of a GetValue() method. Think of the DataSet as an object with properties. Think of the DataSet as a collection of DataPoints, each of which is a key-value pair. 

You submit a DataSet for execution by a RuleSet's execution agent, as follows:-

	myRuleSet.execute(myDataSet);

To create a DataSet:-

	DataSet myDataSet = rmsx.CreateDataSet("<some unique name>");

To create a DataPoint, you first need to create a DataPointSource. This is done by creating a class that extends DataPointSource:-

	private class ConstantDataPointSource : DataPointSource
    {
        string retValue;

        public TestDataPointSource(string retValue)
        {
            this.retValue = retValue;
        }
        public override object GetValue()
        {
            return retValue;
        }
    }

An instance of this class will return the value that was passed to the constructor whenever the GetValue() method is called.

Create the DataPoint as follows:-

	DataPoint myDataPoint = new ConstantDataPointSource("Return This!");

Add the DataPoint to the DataSet:-

	myDataSet.AddDataPoint("DataPoint1", myDataPoint);

Alternatively:-

	myDataSet.AddDataPoint("DataPoint1", new CoonstantDataPointSource("Return This!"));



### Operation

The execution agent that underlies a RuleSet operates in its own thread. When a RuleSets Execute() method is first invoked, the execution agent is created, and the DataSet is passed to the new agent. Thereafter, any further calls to Execute() will result in the DataSet simply being passed to the already running agent.

When a DataSet is ingested by the execution agent, all the Rules will be tested. Once a rule is tested, it will not be tested again, unless it is re-introduced. This happens when a RuleCondition within the rule has a declared dependency on a DataPoint whos DataPointSource has been marked as stale. This is done on the client side, by caling SetStale() on a DataPointSource object. Any Rule that has a dependency on that DataPoint will be re-introduced into the queue of Rules to be tested.

This means that a RuleCondition can be created that depends of the value of a variable or field that will change over time. When the rule is first tested, perhaps the value is in a state that means that the Evaluate() method will return False. However, it may change later. The rule will not be tested again under normal circumstances. But if the variable or field changes values, simply call the SetStale() method on the DataPointSource object, and any and all Rules which have a RuleCondition that depends on its value will be re-tested. This means that the RuleCondition may now return True, and the associated ActionExecutor of Rule will be fired.


### Example

The Rule:-

If a new trade is entered, and the exchange is US, the this order should be sent to broker BRKA. However, if the order is for a LN exchange, then is should be sent to BRKB. Additionally, if the order 


### Coding Style



### Prerequisites

Requires .NET 4.0, but has no other external depedencies.


### Installing

Download the source code, and build the library from source.

### Tests 

NUnit unit tests, as well as integration tests, are included in the project.


### Deployment

Simply distribute the library with any application distribution.

## Authors

* **Richard Clegg** - *Concept, development and samples* - [rikclegg](https://github.com/rikclegg)
* **Terrence Kim** - *Testing and samples* - [tkim](https://github.com/tkim)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

Prof. Deepak Khemani,Department of Computer Science and Engineering, IIT Madras - [Rete Algorithm](https://www.youtube.com/watch?v=XG1sxRcdQZY)