using Xunit;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ICSStudio.StxEditor.ViewModel.Indentation.UnitTests
{
    public class IndentationUnitTests
    {
        public static IEnumerable<object[]> TestDataProvider()
        {
            var data = new List<object[]>
            {
                new object[]
                {
                    @"$if 1 then

                ",
                    "end_if"
                },
                new object[]
                {
                    @"$while 1 do
                ",
                    "end_while"
                },
                new object[]
                {
                    @"$repeat
                  until 1
                ",
                    "end_repeat"
                },
                new object[]
                {
                    @"$case count of
	                  1: 
                      else
                ",
                    "end_case"
                },
                //new object[]
                //{
                //    @"
                //$exp$/*
                //$cur$*/
                //"
                //},
                //new object[]
                //{
                //    @"
                //$exp$(*
                //$cur$*)
                //"
                //},        //Matching annotation keywords are not currently supported
                new object[]
                {
                    @"if 1 then $if 1 then
                ",
                    "end_if"
                },
                new object[]
                {
                    @"if 1 then if 1 then $if 1 then
                ",
                    "end_if"
                },
                new object[]
                {
                    @"if 1 then
                      $if 1 then
                ",
                    "end_if"
                },
                new object[]
                {
                    @"if 1 then
                      if 1 then
                          $if 1 then
                ",
                    "end_if"
                },
                new object[]
                {
                    @"$for count := 1 to 3 do
                ",
                    "end_for"
                },
                new object[]
                {
                    @"$for count := 1 to 4 do
                       if 1 then
                       end_if;
                ",
                    "end_for"
                },
                new object[]
                {
                    @"if
                        if
                      if
                $if
                ",
                    "end_if"
                },
                new object[]
                {
                    @"//忽略闭合块注释中的关键字
                if
                      if
                $if
                /*
                    if
                        if
                */
                ",
                    "end_if"
                },
                new object[]
                {
                    @"//忽略行注释中关键字
                    if
                        if
                $if
                //  if
                ",
                    "end_if"
                },
                new object[]
                {
                    @"$for count := 1 to 5 do
                       if 1 then
                ",
                    "end_for"
                },
                new object[]
                {
                    @"if 1 then
                            $if 1 then
                            elsif 1 then
                    ",
                    "elsif"
                },
                new object[]
                {
                    @"repeat
                            $repeat

                            until 1
                    ",
                    "end_repeat"
                },
                new object[]
                {
                    @"$repeat
                            repeat

                            until 1
                            end_repeat;
                    ",
                    "until"
                },
                new object[]
                {
                    @"  // if
		   // if 
		        if
		$if	// if
				  **** (*(*
				  efef
				  (*(***
wdwwdw
							if
				     if* *)
					 test := 1;
					****  *)
					",
                    "end_if"
                },
                new object[]
                {
                    @"  // if
		   // if 
		        $if
		if	// if
				  **** (*(*
				  efef
				  (*(***
wdwwdw
							if
				     if* *)
					 test := 1;
					****  *)
		end_if;
				",
                    "end_if"
                },
                new object[]
                {
                    @"if // if
							/* if (*  if */		$if //(* /* */ if (* if */ if *) if
										if (*  if *) if      if/*  if  */if
		*)
											
										                                 end_if;
										                     end_if;
										             end_if;
										end_if",
                    "end_if"
                },
                new object[]
                {
                    "",
                    "end_if"
                },
                new object[]
                {
                    @"   $if 1 then;
#region if sefs sef sef sef  sef",
                    "end_if",
                },
                new object[]
                {
                    @"  $if 1 then
#endregion // #endregion if if end_if; /* */;",
                    "end_if"
                },
                new object[]
                {
                    @"  $if 1 then;
		if 1 then;
#region if sefs sef sef sef  sef 
		end_if;
		//
			if 1 then
#endregion // #endregion if if end_if; /* */
			end_if;",
                    "end_if"
                },
                new object[]
                {
                    @"     $if	#region if ",
                    "else",
                },
                new object[]
                {
                    @"  $case	#region if ",
                    "else",
                }
            };

            return data;
        }

        [Theory(DisplayName = "缩进自动匹配测试(模板)")]
        [MemberData(nameof(TestDataProvider))]
        public void GetKeywordScopeLocationUnitTests1(string aboveContext, string endKeyword)
        {
            var matches = Regex.Matches(aboveContext, @"\$");
            var targetOffset = -1;
            if (matches.Count == 1) targetOffset = matches[0].Index;

            var match = Indentation.GetKeywordScopeLocation(aboveContext.Replace("$",""), endKeyword);
            if (match == null)
            {
                Assert.True(targetOffset == -1);
                return;
            }

            Assert.True(match.Index == targetOffset);
        }
    }
}