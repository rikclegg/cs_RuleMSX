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

Add the RuleCondition to the Rule:-

	myRuleCondition.AddRuleCondition(myRuleCondition);

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
            this.parent = parent;
            this.state = state;
        }

        public void Execute(DataSet dataSet)
        {
            this.parent.log("Order " + dataSet.GetDataPoint("OrderNumber").GetValue() + ": " + this.state);
        }
    }

            ruleOrderNew.AddAction(this.rmsx.CreateAction("ShowOrderNew", new ShowOrderState(this, "New Order")));



### Prerequisites

What things you need to install the software and how to install them

```
Give examples
```

### Installing

A step by step series of examples that tell you how to get a development env running

Say what the step will be

```
Give the example
```

And repeat

```
until finished
```

End with an example of getting some data out of the system or using it for a little demo

## Running the tests

Explain how to run the automated tests for this system

### Break down into end to end tests

Explain what these tests test and why

```
Give an example
```

### And coding style tests

Explain what these tests test and why

```
Give an example
```

## Deployment

Add additional notes about how to deploy this on a live system

## Built With

* [Dropwizard](http://www.dropwizard.io/1.0.2/docs/) - The web framework used
* [Maven](https://maven.apache.org/) - Dependency Management
* [ROME](https://rometools.github.io/rome/) - Used to generate RSS Feeds

## Contributing

Please read [CONTRIBUTING.md](https://gist.github.com/PurpleBooth/b24679402957c63ec426) for details on our code of conduct, and the process for submitting pull requests to us.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/your/project/tags). 

## Authors

* **Billie Thompson** - *Initial work* - [PurpleBooth](https://github.com/PurpleBooth)

See also the list of [contributors](https://github.com/your/project/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* Hat tip to anyone whose code was used
* Inspiration
* etc
