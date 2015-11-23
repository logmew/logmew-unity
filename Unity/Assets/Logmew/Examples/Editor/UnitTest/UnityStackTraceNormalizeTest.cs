using System;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Logmew.Editor.Unittest
{
	[TestFixture]
	public class UnityStackTraceNormalizeTest
	{
		[Test]
		public void Test()
		{
			var original = @"UnityEngine.Debug:Log(Object)
LogmewExamples.ServiceInfluxDb.InfluxDbExampleScript:onLogClick() (at Assets/Logmew/Examples/InfluxDbExample/InfluxDbExampleScript.cs:43)
UnityEngine.EventSystems.EventSystem:Update()";
			var expected = @"LogmewExamples.ServiceInfluxDb.InfluxDbExampleScript:onLogClick()@Assets/Logmew/Examples/InfluxDbExample/InfluxDbExampleScript.cs:43 
UnityEngine.EventSystems.EventSystem:Update()";
						
			var actual = StackTraceUnity.Normalize(original);
			Assert.AreEqual(expected, actual);
		}
	}
}
