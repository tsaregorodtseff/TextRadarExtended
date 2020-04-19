using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FuzzySearchDemo.Models
{
    public struct WordsOfStringTableRow
    {
        public char Simbol;
        public int IndexInString;
        public int NumberOfWord;
        public int SizeOfWord;
        public int NumberInWord;
    }

    public class GroupsTableRow
    {
        public int DiagonalIndex;
        public int InitiallIndexInDiagonal;
        public int FinalDiagonalIndex;
        public int GroupSize;

        public int OverGroupSize;
        public bool IncludedInOverGroup; 

        public int GroupStartCoordinate;
        public int GroupEndCoordinate;

        public int NumberOfWordOfSearchString;
        public int StartNumberInSearchStringWord;
        public int WordSizeOfSearchString;

        public bool ThisIsInitialGroupOfSearchStringWord;
        public double CoverageRatioOfWordSearchString;

        public int NumberOfWordOfDataString;
        public int StartNumberInDataStringWord;
        public int WordSizeOfDataString;

        public bool ThisIsInitialGroupOfDataStringWord;
        public double CoverageRatioOfWordDataString;

        public int DistanceToParentGroup;

        public int DepthOfRecursion;
        public bool Disabled;

        public int Copy_GroupSize;
        public int Copy_OverGroupSize;
        public double Copy_CoverageRatioOfWordSearchString;
        public double Copy_CoverageRatioOfWordDataString;
        public int Copy_StartNumberInSearchStringWord;
        public int Copy_StartNumberInDataStringWord;
        public int Copy_GroupStartCoordinate;
        public int Copy_GroupEndCoordinate;

    }

    public class Search
    {

        public string DataString, SearchString, BriefDisplayOfFoundFragments, FullDisplayOfFoundFragments;
        public int[] BriefDisplayedFragments;

        public double Relevance;

        public int Mode; 
        public int Index;


        private bool Opt_СonvertStringsToLowerCase;
        private bool Opt_ReplaceUselessSymbolOfSearchString;

        private int Opt_MinGroupSize;

        private bool Opt_SortTableGroupOneTime;

        private bool Opt_DeleteIntersections;

        private bool Opt_ControlConformityAttributeInitialGroupWord;

        private bool Opt_OverGroups;

        private bool Opt_DeleteIntersectionsGroupsInWordsDatatringAndSearchString;

        private bool Opt_DeleteGroupsOfWordWithoutStartSymbol;

        private bool Opt_DeleteGroupsOfWordWithLowCoverege;

        private double Opt_ThresholdOfCoefficientOfCompositionOfGroupsOfWord;

        private bool Opt_ConsiderCoefficientOfCoveregeWordByGroup;

        private bool Opt_SearchNearestToPreviousGroup;

        private bool Opt_RecursiveCalculationOfSearchResult;

        private double Opt_ThresholdOfCoefficientWhenRecursiveCalculation;
        private bool Opt_QuickCalculationOfRelevance;
        private bool Opt_FormingFullRepresentationOfResult;
        private bool Opt_FormingBriefRepresentationOfResult;
        private int Opt_GroupSizeThresholdLimit;
        private bool Opt_SearchInOneWord;

        private bool Opt_CalculateRelevanceAsAverageOfWords;
        private int Param_Opt_NumberOfWordsOfSearchStringHigherThanThresholdLevel;
        private int Opt_MinNumberOfSimbolsInWordOfSearchString;

        private bool ThisIsUselessSymbol(char Simbol)
        {
            return Simbol == '.'
                || Simbol == ','
                || Simbol == ';'
                || Simbol == '('
                || Simbol == ')'
                || Simbol == '['
                || Simbol == ']'
                || Simbol == '/'
                || Simbol == '\\'
                || Simbol == '-'
                || Simbol == '–'
                || Simbol == '*'
                || Simbol == '»'
                || Simbol == '«'
                || Simbol == '\"'
                || Simbol == ':'
                || Simbol == '?'
                || Simbol == '!'
                || Simbol == '…'
                || Simbol == '\'';
        }

        private void FillTableOfStringWords(ref string SourceString, ref WordsOfStringTableRow[] TableOfStringWords, int StringBorder, bool ItsSerchString = false)
        {

            int NumberOfWord = 0;
            int NumberInWord = 0;
            bool BodyOfWord = false;
            int CurrentWordRowIndex = -1;
            WordsOfStringTableRow TableRow;
            char SymbolOfRow;
            bool EmptySymbol;
            int IndexOfLastRowOfWord;
            int SizeOfWord;

            if (ItsSerchString)
            {
                Param_Opt_NumberOfWordsOfSearchStringHigherThanThresholdLevel = 0;
            }

            for (int i = 0; i <= StringBorder; i++)
            {

                SymbolOfRow = SourceString[i];
                EmptySymbol = SymbolOfRow == ' ' || ThisIsUselessSymbol(SymbolOfRow);

                if (BodyOfWord == false && EmptySymbol == false)
                {
                    BodyOfWord = true;
                    NumberOfWord += 1;
                    NumberInWord = 1;
                }
                else if (BodyOfWord == false && EmptySymbol == true)
                {

                }
                else if (BodyOfWord == true && EmptySymbol == false)
                {
                    NumberInWord += 1;
                }
                else if (BodyOfWord == true && EmptySymbol == true)
                {
                    IndexOfLastRowOfWord = CurrentWordRowIndex;
                    SizeOfWord = TableOfStringWords[CurrentWordRowIndex].NumberInWord;
                    for (int j = 0; j <= SizeOfWord - 1; j++)
                    {
                        TableOfStringWords[IndexOfLastRowOfWord - j].SizeOfWord = SizeOfWord;
                    }
                    BodyOfWord = false;
                    NumberInWord = 0;
                    if(Opt_CalculateRelevanceAsAverageOfWords && ItsSerchString && SizeOfWord >= Opt_MinNumberOfSimbolsInWordOfSearchString)
                    {
                        Param_Opt_NumberOfWordsOfSearchStringHigherThanThresholdLevel++;
                    }
                }

                TableRow = new WordsOfStringTableRow();
                TableRow.Simbol = SymbolOfRow;
                TableRow.IndexInString = i;
                TableRow.NumberOfWord = EmptySymbol == true ? 0 : NumberOfWord;
                TableRow.NumberInWord = NumberInWord;
                TableOfStringWords[i] = TableRow;
                CurrentWordRowIndex = i;

            }

            if (BodyOfWord == true)
            {
                IndexOfLastRowOfWord = CurrentWordRowIndex;
                SizeOfWord = TableOfStringWords[CurrentWordRowIndex].NumberInWord;
                for (int j = 0; j <= SizeOfWord - 1; j++)
                {
                    TableOfStringWords[IndexOfLastRowOfWord - j].SizeOfWord = SizeOfWord;
                }

                if (Opt_CalculateRelevanceAsAverageOfWords && ItsSerchString && SizeOfWord >= Opt_MinNumberOfSimbolsInWordOfSearchString)
                {
                    Param_Opt_NumberOfWordsOfSearchStringHigherThanThresholdLevel++;
                }

            }

        }

        private void RemoveIntersections(ref List<GroupsTableRow> GroupsTable, ref GroupsTableRow BestGroup)
        {

            List<GroupsTableRow> DeletedRows = new List<GroupsTableRow>();

            int StartBestGroup, EndBestGroup, StartGroup, EndGroup, Difference;

            for (int i = 0; i < GroupsTable.Count; i++)
            {

                if (GroupsTable[i] == BestGroup)
                {
                    continue;
                }

                if (BestGroup.NumberOfWordOfSearchString == GroupsTable[i].NumberOfWordOfSearchString)
                {

                    StartBestGroup = BestGroup.StartNumberInSearchStringWord;
                    EndBestGroup = BestGroup.StartNumberInSearchStringWord + BestGroup.GroupSize - 1;
                    StartGroup = GroupsTable[i].StartNumberInSearchStringWord;
                    EndGroup = GroupsTable[i].StartNumberInSearchStringWord + GroupsTable[i].GroupSize - 1;

                    if (StartBestGroup < StartGroup && StartGroup <= EndBestGroup && EndBestGroup <= EndGroup)
                    {

                        Difference = EndBestGroup - StartGroup + 1;

                        var tmp = GroupsTable[i];
                        tmp.InitiallIndexInDiagonal += Difference;
                        tmp.GroupSize -= Difference;
                        tmp.OverGroupSize -= Difference;
                        tmp.GroupStartCoordinate += Difference;
                        tmp.StartNumberInSearchStringWord += Difference;
                        tmp.StartNumberInDataStringWord += Difference;

                        GroupsTable[i] = tmp;

                        if (tmp.GroupSize == 0)
                        {
                            DeletedRows.Add(GroupsTable[i]);
                        }
                        else if (Opt_ConsiderCoefficientOfCoveregeWordByGroup)
                        {
                            tmp.CoverageRatioOfWordSearchString = (double)tmp.OverGroupSize / tmp.WordSizeOfSearchString;
                            tmp.CoverageRatioOfWordDataString = (double)tmp.OverGroupSize / tmp.WordSizeOfDataString;
                        }


                    }

                    else if (StartGroup < StartBestGroup && StartBestGroup <= EndGroup && EndGroup <= EndBestGroup)
                    {

                        Difference = EndGroup - StartBestGroup + 1;

                        var tmp = GroupsTable[i];
                        tmp.FinalDiagonalIndex -= Difference;
                        tmp.GroupSize -= Difference;
                        tmp.OverGroupSize -= Difference;
                        tmp.GroupEndCoordinate -= Difference;

                        GroupsTable[i] = tmp;

                        if (tmp.GroupSize == 0)
                        {
                            DeletedRows.Add(GroupsTable[i]);
                        }
                        else if (Opt_ConsiderCoefficientOfCoveregeWordByGroup)
                        {
                            tmp.CoverageRatioOfWordSearchString = (double)tmp.OverGroupSize / tmp.WordSizeOfSearchString;
                            tmp.CoverageRatioOfWordDataString = (double)tmp.OverGroupSize / tmp.WordSizeOfDataString;
                        }

                    }

                    else if (StartGroup >= StartBestGroup && EndGroup <= EndBestGroup)
                    {

                        DeletedRows.Add(GroupsTable[i]);

                    }

                    else if(Opt_OverGroups && BestGroup.IncludedInOverGroup && StartBestGroup > StartGroup && EndGroup > EndBestGroup)
                    {
                        DeletedRows.Add(GroupsTable[i]);
                    }

                }

            }

            for (int j = 0; j < DeletedRows.Count; j++)
            {
                GroupsTable.Remove(DeletedRows[j]);
            }

            DeletedRows.Clear();

            for (int i = 0; i < GroupsTable.Count; i++)
            {
                if (GroupsTable[i] == BestGroup)
                {
                    continue;
                }

                if (BestGroup.NumberOfWordOfDataString == GroupsTable[i].NumberOfWordOfDataString)
                {

                    StartBestGroup = BestGroup.StartNumberInDataStringWord;
                    EndBestGroup = BestGroup.StartNumberInDataStringWord + BestGroup.GroupSize - 1;
                    StartGroup = GroupsTable[i].StartNumberInDataStringWord;
                    EndGroup = GroupsTable[i].StartNumberInDataStringWord + GroupsTable[i].GroupSize - 1;

                    if (StartBestGroup < StartGroup && StartGroup <= EndBestGroup && EndBestGroup <= EndGroup)
                    {

                        Difference = EndBestGroup - StartGroup + 1;

                        var tmp = GroupsTable[i];
                        tmp.InitiallIndexInDiagonal += Difference;
                        tmp.GroupSize -= Difference;
                        tmp.OverGroupSize -= Difference;
                        tmp.GroupStartCoordinate += Difference;
                        tmp.StartNumberInSearchStringWord += Difference;
                        tmp.StartNumberInDataStringWord += Difference;

                        GroupsTable[i] = tmp;

                        if(tmp.GroupSize == 0)
                        {
                            DeletedRows.Add(GroupsTable[i]);
                        }
                        else if (Opt_ConsiderCoefficientOfCoveregeWordByGroup)
                        {
                            tmp.CoverageRatioOfWordSearchString = (double)tmp.OverGroupSize / tmp.WordSizeOfSearchString;
                            tmp.CoverageRatioOfWordDataString = (double)tmp.OverGroupSize / tmp.WordSizeOfDataString;
                        }

                    }
                    else if(StartGroup < StartBestGroup && StartBestGroup <= EndGroup && EndGroup <= EndBestGroup)
                    {

                        Difference = EndGroup - StartBestGroup + 1;

                        var tmp = GroupsTable[i];
                        tmp.FinalDiagonalIndex -= Difference;
                        tmp.GroupSize -= Difference;
                        tmp.OverGroupSize -= Difference;
                        tmp.GroupEndCoordinate -= Difference;

                        GroupsTable[i] = tmp;

                        if (tmp.GroupSize == 0)
                        {
                            DeletedRows.Add(GroupsTable[i]);
                        }
                        else if(Opt_ConsiderCoefficientOfCoveregeWordByGroup)
                        {
                            tmp.CoverageRatioOfWordSearchString = (double)tmp.OverGroupSize / tmp.WordSizeOfSearchString;
                            tmp.CoverageRatioOfWordDataString = (double)tmp.OverGroupSize / tmp.WordSizeOfDataString;
                        }       

                    }
                    else if (StartGroup >= StartBestGroup && EndGroup <= EndBestGroup)
                    {

                        DeletedRows.Add(GroupsTable[i]);

                    }

                    else if (Opt_OverGroups && BestGroup.IncludedInOverGroup && StartBestGroup > StartGroup && EndGroup > EndBestGroup)
                    {
                        DeletedRows.Add(GroupsTable[i]);
                    }

                }

            }

            for (int j = 0; j < DeletedRows.Count; j++)
            {

                GroupsTable.Remove(DeletedRows[j]);
            }
        }

        private void RemoveIntersectionsOfGroupsBySearchStringAndDataStringWords(ref List<GroupsTableRow> GroupsTable, ref GroupsTableRow BestGroup)
        {

            List<GroupsTableRow> DeletedRows = new List<GroupsTableRow>();

            for (int i = 0; i < GroupsTable.Count; i++)
            {

                if (GroupsTable[i] == BestGroup)
                {
                    continue;
                }

                if (GroupsTable[i].NumberOfWordOfSearchString == BestGroup.NumberOfWordOfSearchString
                        && GroupsTable[i].NumberOfWordOfDataString != BestGroup.NumberOfWordOfDataString)
                {
                    DeletedRows.Add(GroupsTable[i]);
                }

                else if (GroupsTable[i].NumberOfWordOfDataString == BestGroup.NumberOfWordOfDataString
                        && GroupsTable[i].NumberOfWordOfSearchString != BestGroup.NumberOfWordOfSearchString)
                {
                    DeletedRows.Add(GroupsTable[i]);
                }

            }

            for (int j = 0; j < DeletedRows.Count; j++)
            {
                GroupsTable.Remove(DeletedRows[j]);
            }

        }

        private void FormChainOfGroups(
            ref List<GroupsTableRow> GroupsTable, ref List<GroupsTableRow> ResultGroups, int Mode, int DataStringLength, int DepthOfRecursion = 0, GroupsTableRow PreviousGroup = null)
        {

            GroupsTableRow BestGroup = new GroupsTableRow();

            if (GroupsTable.Count > 0)
            {

                if (Opt_SortTableGroupOneTime == true)
                {
                    BestGroup = GroupsTable[0];
                }
                else if(Opt_ConsiderCoefficientOfCoveregeWordByGroup)
                {
                    GroupsTable = GroupsTable
                        .OrderByDescending(x => x.OverGroupSize)
                        .ThenByDescending(x => x.IncludedInOverGroup)
                        .ThenByDescending(x => x.CoverageRatioOfWordSearchString)
                        .ThenByDescending(x => x.CoverageRatioOfWordDataString)
                        .ThenBy(x => x.StartNumberInSearchStringWord)
                        .ThenBy(x => x.StartNumberInDataStringWord)
                        .ThenBy(x => x.DiagonalIndex)
                        .ThenBy(x => x.NumberOfWordOfSearchString)
                        .ToList();
                }
                else
                {
                    GroupsTable = GroupsTable
                        .OrderByDescending(x => x.GroupSize)
                        .ThenBy(x => x.StartNumberInSearchStringWord)
                        .ThenBy(x => x.StartNumberInDataStringWord)
                        .ThenBy(x => x.DiagonalIndex)
                        .ThenBy(x => x.NumberOfWordOfSearchString)
                        .ToList();
                }

                if (!Opt_SearchNearestToPreviousGroup || PreviousGroup == null || GroupsTable.Count == 1)
                {
                    BestGroup = GroupsTable[0];
                }
                else
                {
                    BestGroup = GroupsTable[0];

                    int СoordinateOfEndOfParent = PreviousGroup.GroupEndCoordinate;

                    int СoordinateOfBeginingOfParent = PreviousGroup.GroupStartCoordinate;

                    int InitialDiagonalIndexOfParent = PreviousGroup.InitiallIndexInDiagonal;

                    int MinDistanceToParentGroup = 0;

                    int IndexOfNearestGroup = 0;

                    int СurrentIndex = 0;

                    int EndOfStringCoordinate;
                    int BeginningOfStringCoordinate;
                    int DistanceToParentGroup;

                    for (int i = 0; i < GroupsTable.Count; i++)
                    {

                        if (
                            GroupsTable[i].GroupSize == BestGroup.GroupSize
                            && GroupsTable[i].OverGroupSize == BestGroup.OverGroupSize
                            && GroupsTable[i].InitiallIndexInDiagonal == BestGroup.InitiallIndexInDiagonal
                            && GroupsTable[i].ThisIsInitialGroupOfSearchStringWord == BestGroup.ThisIsInitialGroupOfSearchStringWord
                            && GroupsTable[i].ThisIsInitialGroupOfDataStringWord == BestGroup.ThisIsInitialGroupOfDataStringWord
                            )
                        {
                            EndOfStringCoordinate = GroupsTable[i].GroupEndCoordinate;
                            BeginningOfStringCoordinate = GroupsTable[i].GroupStartCoordinate;

                            if (InitialDiagonalIndexOfParent < GroupsTable[i].InitiallIndexInDiagonal)
                            {
                                if (СoordinateOfEndOfParent < EndOfStringCoordinate)
                                {
                                    DistanceToParentGroup = BeginningOfStringCoordinate - СoordinateOfEndOfParent + GroupsTable[i].GroupSize - 1;
                                }
                                else
                                {

                                    DistanceToParentGroup = DataStringLength - СoordinateOfEndOfParent + DataStringLength + BeginningOfStringCoordinate + GroupsTable[i].GroupSize - 1;

                                }
                            }
                            else
                            {
                                if (СoordinateOfEndOfParent > EndOfStringCoordinate)
                                {
                                    DistanceToParentGroup = СoordinateOfBeginingOfParent - EndOfStringCoordinate + GroupsTable[i].GroupSize - 1;
                                }
                                else
                                {
                                    DistanceToParentGroup = DataStringLength - EndOfStringCoordinate + СoordinateOfBeginingOfParent + DataStringLength;
                                }
                            }

                            if (СurrentIndex == 0)
                            {
                                MinDistanceToParentGroup = DistanceToParentGroup;
                                IndexOfNearestGroup = i;
                            }
                            else
                            {

                                if (DistanceToParentGroup < MinDistanceToParentGroup && DistanceToParentGroup > 0)
                                {
                                    MinDistanceToParentGroup = DistanceToParentGroup;
                                    IndexOfNearestGroup = i;
                                }
                            }
                            СurrentIndex++;
                        }
                        else
                        {
                            break;
                        }

                    }
                    BestGroup = GroupsTable[IndexOfNearestGroup];
                    BestGroup.DistanceToParentGroup = MinDistanceToParentGroup;
                }


            }
            else
            {
                return;
            }

            BestGroup.DepthOfRecursion = DepthOfRecursion;
            ResultGroups.Add(BestGroup);

            if (Opt_DeleteIntersections == true)
            {
                RemoveIntersections(ref GroupsTable, ref BestGroup);
            }


            if (Opt_DeleteIntersectionsGroupsInWordsDatatringAndSearchString)
            {
                RemoveIntersectionsOfGroupsBySearchStringAndDataStringWords(ref GroupsTable, ref BestGroup);
            }

            GroupsTable.Remove(BestGroup);

            FormChainOfGroups(ref GroupsTable, ref ResultGroups, Mode, DataStringLength, DepthOfRecursion, BestGroup);

        }

        private int FindSeparator(int InitialIndexOfDataString, char Separator, int NumberOfSeparator, char Direction)
        {
            int IndexOfSeparator = InitialIndexOfDataString;
            int NumberOfFindSeparators = 0;

            if (Direction == 'L')
            {
                IndexOfSeparator = 0;

                for (int i = InitialIndexOfDataString; i >= 0; i--)
                {

                    if (DataString[i] == Separator && NumberOfFindSeparators < NumberOfSeparator)
                    {
                        NumberOfFindSeparators++;
                    }
                    else if (DataString[i] == Separator && NumberOfFindSeparators == NumberOfSeparator)
                    {
                        IndexOfSeparator = i;
                        NumberOfFindSeparators++;
                        break;
                    }

                }
            }

            if (Direction == 'R')
            {
                IndexOfSeparator = DataString.Length - 1;

                for (int i = InitialIndexOfDataString; i < DataString.Length; i++)
                {

                    if (DataString[i] == Separator && NumberOfFindSeparators < NumberOfSeparator)
                    {
                        NumberOfFindSeparators++;
                    }
                    else if (DataString[i] == Separator && NumberOfFindSeparators == NumberOfSeparator)
                    {
                        IndexOfSeparator = i;
                        NumberOfFindSeparators++;
                        break;
                    }

                }
            }

            return IndexOfSeparator;

        }

        private void FillFoundFragments(ref List<GroupsTableRow> ResultGroups,
                ref Stack<int> BriefDisplayCharactersOfDataString,
                ref Stack<int> BriefFoundCharactersInDataString,
                ref Stack<int> BriefFoundCharactersInDataStringExtended)
        {

            for (int i = 0; i < ResultGroups.Count; i++)
            {

                if(ResultGroups[i].Disabled == true)
                {
                    continue;
                }

                int StartOfDisplayRange = FindSeparator(ResultGroups[i].GroupStartCoordinate, ' ', 2, 'L');
                int EndOfDisplayRange = FindSeparator(ResultGroups[i].GroupEndCoordinate, ' ', 2, 'R');

                for (int j = StartOfDisplayRange; j <= EndOfDisplayRange; j++)
                {
                    BriefDisplayCharactersOfDataString.Push(j);
                }

                for (int j = ResultGroups[i].GroupStartCoordinate; j <= ResultGroups[i].GroupEndCoordinate; j++)
                {
                    if (ResultGroups[i].DepthOfRecursion == 0)
                    {
                        BriefFoundCharactersInDataString.Push(j);
                    }
                    else
                    {
                        BriefFoundCharactersInDataStringExtended.Push(j);
                    }
                }

            }

        }

        public void FormRepresentationsOfResult(ref List<GroupsTableRow> ResultGroups,
                ref Stack<int> BriefDisplayCharactersOfDataString,
                ref Stack<int> BriefFoundCharactersInDataString,
                ref Stack<int> BriefFoundCharactersInDataStringExtended,
                bool FormingFullRepresentationOfResult,
                bool ModeOfExclusionOfPreviouslyFoundResults)
        {
            FillFoundFragments(
                ref ResultGroups,
                ref BriefDisplayCharactersOfDataString,
                ref BriefFoundCharactersInDataString,
                ref BriefFoundCharactersInDataStringExtended);

            BriefDisplayedFragments = BriefDisplayCharactersOfDataString.ToArray<int>();
            Array.Sort<int>(BriefDisplayedFragments);

            var hashset = new HashSet<int>();

            foreach (var x in BriefDisplayedFragments)
                if (!hashset.Contains(x))
                    hashset.Add(x);

            Array.Resize(ref BriefDisplayedFragments, hashset.Count);
            BriefDisplayedFragments = hashset.ToArray();

            BriefDisplayOfFoundFragments = "";
            int BriefDisplayedFragmentsLength = BriefDisplayedFragments.Length;

            int OriginalDataStringBorder = this.DataString.Length - 1;

            if (BriefDisplayedFragmentsLength > 0)
            {
                if (BriefDisplayedFragments[0] != 0)
                    BriefDisplayOfFoundFragments += "...";

                for (int i = 0; i < BriefDisplayedFragmentsLength; i++)
                {

                    if (BriefDisplayedFragments[i] > OriginalDataStringBorder)
                    {
                        continue;
                    }

                    if (BriefFoundCharactersInDataString.Contains<int>(BriefDisplayedFragments[i]))
                    {
                        BriefDisplayOfFoundFragments += "<span class=\"findefragments\">" + this.DataString[BriefDisplayedFragments[i]] + "</span>";
                    }
                    else
                    {
                        BriefDisplayOfFoundFragments += this.DataString[BriefDisplayedFragments[i]];
                    }

                    if (i < BriefDisplayedFragmentsLength - 1 && BriefDisplayedFragments[i] + 1 != BriefDisplayedFragments[i + 1])
                        BriefDisplayOfFoundFragments += "...";
                }

                if (BriefDisplayedFragments[BriefDisplayedFragmentsLength - 1] < OriginalDataStringBorder)
                    BriefDisplayOfFoundFragments += "...";
            }

            if (FormingFullRepresentationOfResult == true && ModeOfExclusionOfPreviouslyFoundResults == false)
            {
                FullDisplayOfFoundFragments = "";

                for (int i = 0; i <= OriginalDataStringBorder; i++)
                {

                    if (BriefFoundCharactersInDataString.Contains<int>(i))
                    {
                        {
                            FullDisplayOfFoundFragments += "<span class=\"select1\">" + this.DataString[i] + "</span>";
                        }
                    }
                    else if (BriefFoundCharactersInDataStringExtended.Contains<int>(i))
                    {
                        FullDisplayOfFoundFragments += "<span class=\"select2\">" + this.DataString[i] + "</span>";
                    }

                    else
                    {
                        FullDisplayOfFoundFragments += this.DataString[i];
                    }
                }
            }

        }

        private void DeleteGroupsOfWordsOfDataStringWithoutFirstSymbolAndWithLowCoverage(ref List<GroupsTableRow> ResultGroups, int DepthOfRecursion)
        {

            ResultGroups = ResultGroups.OrderBy(x => x.NumberOfWordOfDataString).ToList();

            int CurrentNumberOfWord = -1;
            int CurrentSizeOfWord = 0;
            int CurrentSumGroupSize = 0;
            int CurrentSumThisIsInitialGroupOfWord = 0;
            List<GroupsTableRow> CurrentWordGroups = new List<GroupsTableRow>();

            List<GroupsTableRow> DeletedRows = new List<GroupsTableRow>();

            for (int i = 0; i < ResultGroups.Count; i++)
            {
                if (ResultGroups[i].DepthOfRecursion != DepthOfRecursion)
                {
                    continue;
                }

                if (ResultGroups[i].NumberOfWordOfDataString != CurrentNumberOfWord)
                {
                    if(CurrentNumberOfWord != -1)
                    {
                        if (
                            (Opt_DeleteGroupsOfWordWithoutStartSymbol && CurrentSumThisIsInitialGroupOfWord == 0) 
                            || 
                            (Opt_DeleteGroupsOfWordWithLowCoverege && (double)CurrentSumGroupSize / CurrentSizeOfWord < Opt_ThresholdOfCoefficientOfCompositionOfGroupsOfWord)
                            )
                        {
                            foreach (var CurrentWordGroup in CurrentWordGroups)
                            {
                                DeletedRows.Add(CurrentWordGroup);
                            }
                        }
                    }
                    CurrentNumberOfWord = ResultGroups[i].NumberOfWordOfDataString;
                    CurrentSizeOfWord = ResultGroups[i].WordSizeOfDataString * ResultGroups[i].WordSizeOfDataString;
                    CurrentSumGroupSize = ResultGroups[i].GroupSize * ResultGroups[i].GroupSize;
                    CurrentSumThisIsInitialGroupOfWord = ResultGroups[i].ThisIsInitialGroupOfDataStringWord ? 1:0;
                    CurrentWordGroups.Clear();
                    CurrentWordGroups.Add(ResultGroups[i]);
                }
                else
                {
                    CurrentSumGroupSize += ResultGroups[i].GroupSize * ResultGroups[i].GroupSize;
                    CurrentSumThisIsInitialGroupOfWord += ResultGroups[i].ThisIsInitialGroupOfDataStringWord ? 1 : 0;
                    CurrentWordGroups.Add(ResultGroups[i]);
                }
            }
            if (
                (Opt_DeleteGroupsOfWordWithoutStartSymbol && CurrentSumThisIsInitialGroupOfWord == 0)
                ||
                (Opt_DeleteGroupsOfWordWithLowCoverege && (double)CurrentSumGroupSize / CurrentSizeOfWord < Opt_ThresholdOfCoefficientOfCompositionOfGroupsOfWord)
                )
            {
                foreach (var CurrentWordGroup in CurrentWordGroups)
                {
                    DeletedRows.Add(CurrentWordGroup);
                }
            }

            ResultGroups = ResultGroups.OrderBy(x => x.NumberOfWordOfSearchString).ToList();

            CurrentNumberOfWord = -1;
            CurrentSizeOfWord = 0;
            CurrentSumGroupSize = 0;
            CurrentSumThisIsInitialGroupOfWord = 0;
            CurrentWordGroups = new List<GroupsTableRow>();

            for (int i = 0; i < ResultGroups.Count; i++)
            {
                if (ResultGroups[i].DepthOfRecursion != DepthOfRecursion)
                {
                    continue;
                }

                if (ResultGroups[i].NumberOfWordOfSearchString != CurrentNumberOfWord)
                {
                    if (CurrentNumberOfWord != -1)
                    {
                        if (
                            (Opt_DeleteGroupsOfWordWithoutStartSymbol && CurrentSumThisIsInitialGroupOfWord == 0)
                            ||
                            (Opt_DeleteGroupsOfWordWithLowCoverege && (double)CurrentSumGroupSize / CurrentSizeOfWord < Opt_ThresholdOfCoefficientOfCompositionOfGroupsOfWord)
                            )
                        {
                            foreach (var CurrentWordGroup in CurrentWordGroups)
                            {
                                DeletedRows.Add(CurrentWordGroup);
                            }
                        }
                    }
                    CurrentNumberOfWord = ResultGroups[i].NumberOfWordOfSearchString;
                    CurrentSizeOfWord = ResultGroups[i].WordSizeOfSearchString * ResultGroups[i].WordSizeOfSearchString;
                    CurrentSumGroupSize = ResultGroups[i].GroupSize * ResultGroups[i].GroupSize;
                    CurrentSumThisIsInitialGroupOfWord = ResultGroups[i].ThisIsInitialGroupOfSearchStringWord ? 1 : 0;
                    CurrentWordGroups.Clear();
                    CurrentWordGroups.Add(ResultGroups[i]);
                }
                else
                {
                    CurrentSumGroupSize += ResultGroups[i].GroupSize * ResultGroups[i].GroupSize;
                    CurrentSumThisIsInitialGroupOfWord += ResultGroups[i].ThisIsInitialGroupOfSearchStringWord ? 1 : 0;
                    CurrentWordGroups.Add(ResultGroups[i]);
                }
            }
            if (
                (Opt_DeleteGroupsOfWordWithoutStartSymbol && CurrentSumThisIsInitialGroupOfWord == 0)
                ||
                (Opt_DeleteGroupsOfWordWithLowCoverege && (double)CurrentSumGroupSize / CurrentSizeOfWord < Opt_ThresholdOfCoefficientOfCompositionOfGroupsOfWord)
                )
            {
                foreach (var CurrentWordGroup in CurrentWordGroups)
                {
                    DeletedRows.Add(CurrentWordGroup);
                }
            }

            for (int j = 0; j < DeletedRows.Count; j++)
            {
                DeletedRows[j].Disabled = true;
            }

        }

        private double CalculateCoefficient(
            ref WordsOfStringTableRow[] TableOfStringWords, int SearchStringLength, int DataStringLength, ref List<GroupsTableRow> ResultGroups, int Mode, int DepthOfRecursion)
        {

            int GroupsOfResultSum = 0;
            int GroupSizeThresholdLimit;

            if (Opt_QuickCalculationOfRelevance && !Opt_CalculateRelevanceAsAverageOfWords)
            {

                for (int i = 0; i < ResultGroups.Count; i++)
                {
                    if (ResultGroups[i].Disabled == true)
                    {
                        continue;
                    }
                    if (Opt_GroupSizeThresholdLimit > 0)
                    {
                        GroupSizeThresholdLimit = ResultGroups[i].GroupSize > Opt_GroupSizeThresholdLimit ? Opt_GroupSizeThresholdLimit : ResultGroups[i].GroupSize;
                    }
                    else
                    {
                        GroupSizeThresholdLimit = ResultGroups[i].GroupSize;
                    }
                    GroupsOfResultSum += GroupSizeThresholdLimit * GroupSizeThresholdLimit;
                }

                return GroupsOfResultSum;

            }

            if (Opt_SearchInOneWord == true)
            {

                for (int i = 0; i < ResultGroups.Count; i++)
                {
                    if (ResultGroups[i].Disabled == true)
                    {
                        continue;
                    }

                    GroupsOfResultSum += ResultGroups[i].GroupSize * ResultGroups[i].GroupSize;

                }

                var SquareOfDataStringLength = DataStringLength * DataStringLength;
                return SquareOfDataStringLength >= GroupsOfResultSum ? Math.Sqrt((double)GroupsOfResultSum / SquareOfDataStringLength)
                    : Math.Sqrt((double)SquareOfDataStringLength / GroupsOfResultSum);

            }

            if (Opt_CalculateRelevanceAsAverageOfWords == true)
            {

                List<GroupsTableRow> ResultGroupsOrdering = ResultGroups.OrderBy(x => x.NumberOfWordOfSearchString).ToList();

                double СуммаПоВсемГруппам = 0;
                double СуммаПоГруппамСлова = 0;
                int ТекущийНомерСловаСтрокиПоиска = 0;
                int ТекущийРазмерСловаСтрокиПоиска = 0;
                bool ПервыйПроход = true;

                int MinCoordinate_ = DataStringLength - 1;
                int MaxCoordinate_ = 0;

                for (int Индекс = 0; Индекс < ResultGroupsOrdering.Count; Индекс++)
                {

                    if (ResultGroupsOrdering[Индекс].Disabled == true)
                    {
                        continue;
                    }

                    if (ResultGroupsOrdering[Индекс].WordSizeOfSearchString < Opt_MinNumberOfSimbolsInWordOfSearchString)
                    {
                        continue;
                    }

                    if(ПервыйПроход == true)
                    {

                        ПервыйПроход = false;
                        ТекущийНомерСловаСтрокиПоиска = ResultGroupsOrdering[Индекс].NumberOfWordOfSearchString;
                        ТекущийРазмерСловаСтрокиПоиска = ResultGroupsOrdering[Индекс].WordSizeOfSearchString;
                        СуммаПоГруппамСлова = ResultGroupsOrdering[Индекс].GroupSize * ResultGroupsOrdering[Индекс].GroupSize;

                    }
                    else if(ТекущийНомерСловаСтрокиПоиска != ResultGroupsOrdering[Индекс].NumberOfWordOfSearchString)
                    {
                        СуммаПоВсемГруппам += Math.Sqrt(СуммаПоГруппамСлова / (ТекущийРазмерСловаСтрокиПоиска * ТекущийРазмерСловаСтрокиПоиска));

                        ТекущийНомерСловаСтрокиПоиска = ResultGroupsOrdering[Индекс].NumberOfWordOfSearchString;
                        ТекущийРазмерСловаСтрокиПоиска = ResultGroupsOrdering[Индекс].WordSizeOfSearchString;

                        СуммаПоГруппамСлова = ResultGroupsOrdering[Индекс].GroupSize * ResultGroupsOrdering[Индекс].GroupSize;

                    }
                    else
                    {
                        СуммаПоГруппамСлова += ResultGroupsOrdering[Индекс].GroupSize * ResultGroupsOrdering[Индекс].GroupSize;
                    }

                }


                if (СуммаПоГруппамСлова == 0 || ТекущийРазмерСловаСтрокиПоиска == 0)
                {
                    return 0;
                }

                СуммаПоВсемГруппам += Math.Sqrt(СуммаПоГруппамСлова / (ТекущийРазмерСловаСтрокиПоиска * ТекущийРазмерСловаСтрокиПоиска));

                if (Opt_QuickCalculationOfRelevance)
                {
                    return СуммаПоВсемГруппам;
                }
                else
                {

                    for (int Индекс = 0; Индекс < ResultGroups.Count; Индекс++)
                    {
                        if (!ResultGroups[Индекс].Disabled)
                        {
                            if (MinCoordinate_ > ResultGroups[Индекс].GroupStartCoordinate)
                            {
                                MinCoordinate_ = ResultGroups[Индекс].GroupStartCoordinate;
                            }
                            if (MaxCoordinate_ < ResultGroups[Индекс].GroupEndCoordinate)
                            {
                                MaxCoordinate_ = ResultGroups[Индекс].GroupEndCoordinate;
                            }
                        }
                    }

                    int ChainOfGroupsLength_ = MaxCoordinate_ - MinCoordinate_ + 1;
                    double CoefficientOfSize_ = СуммаПоВсемГруппам == 0 ? 0.0 : 
                        ChainOfGroupsLength_ < SearchStringLength ? (double)ChainOfGroupsLength_ / SearchStringLength : (double)SearchStringLength / ChainOfGroupsLength_;

                     return 0.9 * ((double)СуммаПоВсемГруппам / Param_Opt_NumberOfWordsOfSearchStringHigherThanThresholdLevel)
                        + 0.1 * CoefficientOfSize_;
                }

            }

            int SearchStringSum = 0;
            int СurrentNumberOfWord = 0;

            for (int i = 0; i < TableOfStringWords.Length; i++)
            {
                if (TableOfStringWords[i].NumberOfWord != СurrentNumberOfWord && TableOfStringWords[i].NumberOfWord != 0)
                {
                    СurrentNumberOfWord = TableOfStringWords[i].NumberOfWord;
                    if (Opt_GroupSizeThresholdLimit > 0)
                    {
                        GroupSizeThresholdLimit = TableOfStringWords[i].SizeOfWord > Opt_GroupSizeThresholdLimit ? Opt_GroupSizeThresholdLimit : TableOfStringWords[i].SizeOfWord;
                    }
                    else
                    {
                        GroupSizeThresholdLimit = TableOfStringWords[i].SizeOfWord;
                    }
                    SearchStringSum += GroupSizeThresholdLimit * GroupSizeThresholdLimit;
                }
            }

            int MinCoordinate = DataStringLength - 1;
            int MaxCoordinate = 0;

            for (int i = 0; i < ResultGroups.Count; i++)
            {
                if(ResultGroups[i].DepthOfRecursion != DepthOfRecursion)
                {
                    continue;
                }
                if (ResultGroups[i].Disabled == true)
                {
                    continue;
                }
                if (MinCoordinate > ResultGroups[i].GroupStartCoordinate)
                {
                    MinCoordinate = ResultGroups[i].GroupStartCoordinate;
                }
                if (MaxCoordinate < ResultGroups[i].GroupEndCoordinate)
                {
                    MaxCoordinate = ResultGroups[i].GroupEndCoordinate;
                }

                if (Opt_GroupSizeThresholdLimit > 0)
                {
                    GroupSizeThresholdLimit = ResultGroups[i].GroupSize > Opt_GroupSizeThresholdLimit ? Opt_GroupSizeThresholdLimit : ResultGroups[i].GroupSize;
                }
                else
                {
                    GroupSizeThresholdLimit = ResultGroups[i].GroupSize;
                }
                GroupsOfResultSum += GroupSizeThresholdLimit * GroupSizeThresholdLimit;
            }

            int ChainOfGroupsLength = MaxCoordinate - MinCoordinate + 1;
            double CoefficientOfMatchSum = SearchStringSum == 0 ? 0.0 : Math.Sqrt((double)GroupsOfResultSum / SearchStringSum);
            double CoefficientOfSize = CoefficientOfMatchSum == 0 ? 0.0 : ChainOfGroupsLength < SearchStringLength ? (double)ChainOfGroupsLength / SearchStringLength : (double)SearchStringLength / ChainOfGroupsLength;

            return 0.9 * CoefficientOfMatchSum + 0.1 * CoefficientOfSize;

        }


        public int CalculateRelevance(string Version = "")
        {

            if (Mode == 0)
            {
                if (Version == "Base")
                {
                    Opt_MinGroupSize = 2;
                    Opt_DeleteIntersections = true;
                    Opt_DeleteIntersectionsGroupsInWordsDatatringAndSearchString = false;
                    Opt_ControlConformityAttributeInitialGroupWord = false;
                    Opt_QuickCalculationOfRelevance = true;
                    Opt_FormingFullRepresentationOfResult = false;
                    Opt_FormingBriefRepresentationOfResult = false;

                    Opt_СonvertStringsToLowerCase = true;
                    Opt_ReplaceUselessSymbolOfSearchString = true;
                    Opt_SortTableGroupOneTime = false;

                    Opt_OverGroups = false;

                    Opt_DeleteGroupsOfWordWithoutStartSymbol = false;
                    Opt_DeleteGroupsOfWordWithLowCoverege = false;
                    Opt_ThresholdOfCoefficientOfCompositionOfGroupsOfWord = 0;
                    Opt_ConsiderCoefficientOfCoveregeWordByGroup = false;
                    Opt_SearchNearestToPreviousGroup = false;
                    Opt_RecursiveCalculationOfSearchResult = false;

                    Opt_GroupSizeThresholdLimit = 0;
                    Opt_SearchInOneWord = false;
                    Opt_CalculateRelevanceAsAverageOfWords = false;
                    Opt_MinNumberOfSimbolsInWordOfSearchString = 0;
                }
                else
                {
                    Opt_СonvertStringsToLowerCase = true;
                    Opt_ReplaceUselessSymbolOfSearchString = true;
                    Opt_MinGroupSize = 2;
                    Opt_SortTableGroupOneTime = false;
                    Opt_DeleteIntersections = true;
                    Opt_ControlConformityAttributeInitialGroupWord = true;
                    Opt_OverGroups = true;
                    Opt_DeleteIntersectionsGroupsInWordsDatatringAndSearchString = true;
                    Opt_DeleteGroupsOfWordWithoutStartSymbol = true;
                    Opt_DeleteGroupsOfWordWithLowCoverege = true;
                    Opt_ThresholdOfCoefficientOfCompositionOfGroupsOfWord = 0.27;
                    Opt_ConsiderCoefficientOfCoveregeWordByGroup = true;
                    Opt_SearchNearestToPreviousGroup = false;
                    Opt_RecursiveCalculationOfSearchResult = false;
                    Opt_QuickCalculationOfRelevance = true;
                    Opt_FormingFullRepresentationOfResult = false;
                    Opt_FormingBriefRepresentationOfResult = false;
                    Opt_GroupSizeThresholdLimit = 0;
                    Opt_SearchInOneWord = false;
                    Opt_CalculateRelevanceAsAverageOfWords = true;
                    Opt_MinNumberOfSimbolsInWordOfSearchString = 2;
                }
            }
            else if (Mode == 1)
            {
                if (Version == "Base")
                {
                    Opt_MinGroupSize = 1;
                    Opt_DeleteIntersections = true;
                    Opt_DeleteIntersectionsGroupsInWordsDatatringAndSearchString = false;
                    Opt_ControlConformityAttributeInitialGroupWord = false;
                    Opt_QuickCalculationOfRelevance = false;
                    Opt_FormingFullRepresentationOfResult = false;
                    Opt_FormingBriefRepresentationOfResult = true;

                    Opt_СonvertStringsToLowerCase = true;
                    Opt_ReplaceUselessSymbolOfSearchString = true;

                    Opt_SortTableGroupOneTime = false;

                    Opt_OverGroups = false;

                    Opt_DeleteGroupsOfWordWithoutStartSymbol = false;
                    Opt_DeleteGroupsOfWordWithLowCoverege = false;
                    Opt_ThresholdOfCoefficientOfCompositionOfGroupsOfWord = 0;
                    Opt_ConsiderCoefficientOfCoveregeWordByGroup = false;
                    Opt_SearchNearestToPreviousGroup = false;
                    Opt_RecursiveCalculationOfSearchResult = false;

                    Opt_GroupSizeThresholdLimit = 0;
                    Opt_SearchInOneWord = false;
                    Opt_CalculateRelevanceAsAverageOfWords = false;
                    Opt_MinNumberOfSimbolsInWordOfSearchString = 0;
                }
                else
                {
                    Opt_СonvertStringsToLowerCase = true;
                    Opt_ReplaceUselessSymbolOfSearchString = true;
                    Opt_MinGroupSize = 1;
                    Opt_SortTableGroupOneTime = false;
                    Opt_DeleteIntersections = true;
                    Opt_ControlConformityAttributeInitialGroupWord = true;
                    Opt_OverGroups = true;
                    Opt_DeleteIntersectionsGroupsInWordsDatatringAndSearchString = true;
                    Opt_DeleteGroupsOfWordWithoutStartSymbol = true;
                    Opt_DeleteGroupsOfWordWithLowCoverege = true;
                    Opt_ThresholdOfCoefficientOfCompositionOfGroupsOfWord = 0.27;
                    Opt_ConsiderCoefficientOfCoveregeWordByGroup = true;
                    Opt_SearchNearestToPreviousGroup = false;
                    Opt_RecursiveCalculationOfSearchResult = false;
                    Opt_QuickCalculationOfRelevance = false;
                    Opt_FormingFullRepresentationOfResult = false;
                    Opt_FormingBriefRepresentationOfResult = true;
                    Opt_GroupSizeThresholdLimit = 0;
                    Opt_SearchInOneWord = false;
                    Opt_CalculateRelevanceAsAverageOfWords = true;
                    Opt_MinNumberOfSimbolsInWordOfSearchString = 2;
                }
            }
            else if (Mode == 2)
            {
                if (Version == "Base")
                {

                    Opt_MinGroupSize = 1;
                    Opt_DeleteIntersections = true;
                    Opt_DeleteIntersectionsGroupsInWordsDatatringAndSearchString = false;
                    Opt_ControlConformityAttributeInitialGroupWord = false;
                    Opt_QuickCalculationOfRelevance = false;
                    Opt_FormingFullRepresentationOfResult = true;
                    Opt_FormingBriefRepresentationOfResult = true;

                    Opt_СonvertStringsToLowerCase = true;
                    Opt_ReplaceUselessSymbolOfSearchString = true;

                    Opt_SortTableGroupOneTime = false;

                    Opt_OverGroups = false;

                    Opt_DeleteGroupsOfWordWithoutStartSymbol = false;
                    Opt_DeleteGroupsOfWordWithLowCoverege = false;
                    Opt_ThresholdOfCoefficientOfCompositionOfGroupsOfWord = 0;
                    Opt_ConsiderCoefficientOfCoveregeWordByGroup = false;
                    Opt_SearchNearestToPreviousGroup = false;
                    Opt_RecursiveCalculationOfSearchResult = false;

                    Opt_GroupSizeThresholdLimit = 0;
                    Opt_SearchInOneWord = false;
                    Opt_CalculateRelevanceAsAverageOfWords = false;
                    Opt_MinNumberOfSimbolsInWordOfSearchString = 0;
                }
                else
                {
                    Opt_СonvertStringsToLowerCase = true;
                    Opt_ReplaceUselessSymbolOfSearchString = true;
                    Opt_MinGroupSize = 1;
                    Opt_SortTableGroupOneTime = false;
                    Opt_DeleteIntersections = true;
                    Opt_ControlConformityAttributeInitialGroupWord = true;
                    Opt_OverGroups = true;
                    Opt_DeleteIntersectionsGroupsInWordsDatatringAndSearchString = true;
                    Opt_DeleteGroupsOfWordWithoutStartSymbol = true;
                    Opt_DeleteGroupsOfWordWithLowCoverege = true;
                    Opt_ThresholdOfCoefficientOfCompositionOfGroupsOfWord = 0.27;
                    Opt_ConsiderCoefficientOfCoveregeWordByGroup = true;
                    Opt_SearchNearestToPreviousGroup = false;
                    Opt_RecursiveCalculationOfSearchResult = true;
                    Opt_ThresholdOfCoefficientWhenRecursiveCalculation = 0.19;
                    Opt_QuickCalculationOfRelevance = false;
                    Opt_FormingFullRepresentationOfResult = true;
                    Opt_FormingBriefRepresentationOfResult = true;
                    Opt_GroupSizeThresholdLimit = 0;
                    Opt_SearchInOneWord = false;
                }
            }
            else if (Mode == 3)
            {

                Opt_СonvertStringsToLowerCase = true;
                Opt_ReplaceUselessSymbolOfSearchString = true;
                Opt_MinGroupSize = 1;
                Opt_SortTableGroupOneTime = false;
                Opt_DeleteIntersections = true;
                Opt_ControlConformityAttributeInitialGroupWord = false;
                Opt_OverGroups = false;
                Opt_DeleteIntersectionsGroupsInWordsDatatringAndSearchString = false;
                Opt_DeleteGroupsOfWordWithoutStartSymbol = false;
                Opt_DeleteGroupsOfWordWithLowCoverege = false;
                Opt_ThresholdOfCoefficientOfCompositionOfGroupsOfWord = 0.0;
                Opt_ConsiderCoefficientOfCoveregeWordByGroup = false;
                Opt_SearchNearestToPreviousGroup = false;
                Opt_RecursiveCalculationOfSearchResult = false;
                Opt_ThresholdOfCoefficientWhenRecursiveCalculation = 0.0;
                Opt_QuickCalculationOfRelevance = false;
                Opt_FormingFullRepresentationOfResult = true;
                Opt_FormingBriefRepresentationOfResult = true;
                Opt_GroupSizeThresholdLimit = 0;
                Opt_SearchInOneWord = false;

            }
            else if (Mode == 4)
            {
                Opt_СonvertStringsToLowerCase = false;
                Opt_ReplaceUselessSymbolOfSearchString = false;
                Opt_MinGroupSize = 1;
                Opt_SortTableGroupOneTime = false;
                Opt_DeleteIntersections = true;
                Opt_ControlConformityAttributeInitialGroupWord = true;
                Opt_OverGroups = false;
                Opt_DeleteIntersectionsGroupsInWordsDatatringAndSearchString = false;
                Opt_DeleteGroupsOfWordWithoutStartSymbol = false;
                Opt_DeleteGroupsOfWordWithLowCoverege = false;
                Opt_ThresholdOfCoefficientOfCompositionOfGroupsOfWord = 0.0;
                Opt_ConsiderCoefficientOfCoveregeWordByGroup = false;
                Opt_SearchNearestToPreviousGroup = false;
                Opt_RecursiveCalculationOfSearchResult = false;
                Opt_QuickCalculationOfRelevance = false;
                Opt_FormingFullRepresentationOfResult = false;
                Opt_FormingBriefRepresentationOfResult = false;
                Opt_GroupSizeThresholdLimit = 0;
                Opt_SearchInOneWord = true;
                Opt_CalculateRelevanceAsAverageOfWords = false;
            }


            List<GroupsTableRow> ResultGroups = new List<GroupsTableRow>();
            CalculateRelevancePrivate(ref ResultGroups);
            return 0;
        }


        private int CalculateRelevancePrivate
            (
                ref List<GroupsTableRow> ResultGroups,
                bool ModeOfExclusionOfPreviouslyFoundResults = false,
                List<GroupsTableRow> GroupsTable = null,
                int DepthOfRecursion = 0,
                WordsOfStringTableRow[] TableOfSearchStringWords = null,
                int SearchStringLength = 0,
                int DataStringLength = 0,
                string DataString = ""
            )

        {

            if(ModeOfExclusionOfPreviouslyFoundResults == true)
            {
                goto LabelResultsExclusionMode;
            }


            if (this.SearchString == null || this.DataString == null)
            {
                Relevance = 0.0;
                return 0;
            }

            string SearchString = this.SearchString;
            DataString = this.DataString;
            SearchStringLength = SearchString.Length;
            DataStringLength = DataString.Length;

            int SearchStringBorder;
            int DataStringBorder;


            if (Opt_СonvertStringsToLowerCase)
            {
                SearchString = SearchString.ToLower();
                DataString = DataString.ToLower();
            }

            if (SearchStringLength == 0 || DataStringLength == 0 || SearchStringLength > 200 || DataStringLength > 3000)
            {
                Relevance = 0.0;
                return 0;
            }

            string StringBuffer = "";
            if (Opt_ReplaceUselessSymbolOfSearchString == true)
            {
                for (int i = 0; i < SearchStringLength; i++)
                {
                    var SymbolOfRow = SearchString[i];
                    if (SymbolOfRow == ' ' || ThisIsUselessSymbol(SymbolOfRow))
                    {
                        SymbolOfRow = ' ';
                    }
                    StringBuffer += SymbolOfRow;
                }
                SearchString = StringBuffer;
            }

            if (DataStringLength < SearchStringLength)
            {
                StringBuffer = "";
                for (int i = 1; i <= SearchStringLength - DataStringLength; i++)
                    StringBuffer += '♦';
                DataString += StringBuffer;
                DataStringLength = DataString.Length;
            }

            SearchStringBorder = SearchStringLength - 1;
            DataStringBorder = DataStringLength - 1;

            bool[] MatchMatrix = new bool[DataStringLength * SearchStringLength];

            for (int i = 0; i <= DataStringBorder; i++)
            {
                for (int j = 0; j <= SearchStringBorder; j++)
                {
                    if (
                        (SearchString[j] == DataString[i]) && SearchString[j] != ' '
                       )
                    {
                        MatchMatrix[SearchStringLength * i + j] = true;
                    }
                    else
                    {
                        MatchMatrix[SearchStringLength * i + j] = false;
                    }
                }
            }

            int DiagonalBorder;
            int StartIndexDiagonal;

            DiagonalBorder = DataStringBorder + SearchStringBorder;

            bool[] DiagonalsValueMatrix = new bool[(DiagonalBorder + 1) * SearchStringLength];

            for (int i = 0; i <= DiagonalBorder; i++)
            {
                for (int j = 0; j <= SearchStringBorder; j++)
                {
                    StartIndexDiagonal = i - SearchStringBorder + j;
                    if (StartIndexDiagonal >= 0 && StartIndexDiagonal <= DataStringBorder)
                        DiagonalsValueMatrix[SearchStringLength * i + j] = MatchMatrix[SearchStringLength * StartIndexDiagonal + j];
                    else
                        DiagonalsValueMatrix[SearchStringLength * i + j] = false;
                }
            }

            WordsOfStringTableRow[] TableOfDataStringWords = new WordsOfStringTableRow[DataStringLength];
            FillTableOfStringWords(ref DataString, ref TableOfDataStringWords, DataStringBorder);

            TableOfSearchStringWords = new WordsOfStringTableRow[SearchStringLength];
            FillTableOfStringWords(ref SearchString, ref TableOfSearchStringWords, SearchStringBorder, true);

            GroupsTable = new List<GroupsTableRow>();
            GroupsTableRow NewListItem;

            int CurrentGroupSize;

            int OverGroupSize = 0;
            Stack<int> OverGroupsIndexes = new Stack<int>();

            int CurrentNumberOfWordsOfSearchString = 0;
            int CurrentNumberOfWordsOfDataString = 0;


            for (int DiagonalIndex = 0; DiagonalIndex <= DiagonalBorder; DiagonalIndex++)
            {

                CurrentGroupSize = 0;

                if (Opt_OverGroups == true)
                {
                    OverGroupSize = 0;
                    OverGroupsIndexes.Clear();
                    CurrentNumberOfWordsOfSearchString = 0;
                    CurrentNumberOfWordsOfDataString = 0;
                }

                for (int PositionInDiagonalIndex = 0; PositionInDiagonalIndex <= SearchStringBorder; PositionInDiagonalIndex++)
                {

                    if (DiagonalsValueMatrix[SearchStringLength * DiagonalIndex + PositionInDiagonalIndex] == true)

                    {
                        CurrentGroupSize += 1;
                    }
                    else
                    {

                        if (CurrentGroupSize >= Opt_MinGroupSize)
                        {

                            var InitiallIndexInDiagonal = PositionInDiagonalIndex - CurrentGroupSize;
                            var WordsTableRowSearchString = TableOfSearchStringWords[InitiallIndexInDiagonal];
                            var ThisIsInitialGroupOfSearchStringWord = WordsTableRowSearchString.NumberInWord == 1;
                            var WordsTableRowDataString = TableOfDataStringWords[DiagonalIndex + InitiallIndexInDiagonal - SearchStringLength + 1];
                            var ThisIsInitialGroupOfDataStringWord = WordsTableRowDataString.NumberInWord == 1;

                            if (Opt_ControlConformityAttributeInitialGroupWord == false || 
                                ThisIsInitialGroupOfSearchStringWord == ThisIsInitialGroupOfDataStringWord)
                            {

                                NewListItem = new GroupsTableRow();

                                NewListItem.DiagonalIndex = DiagonalIndex;
                                NewListItem.InitiallIndexInDiagonal = InitiallIndexInDiagonal;
                                NewListItem.FinalDiagonalIndex = PositionInDiagonalIndex - 1;
                                NewListItem.GroupSize = CurrentGroupSize;
                                NewListItem.OverGroupSize = CurrentGroupSize;

                                NewListItem.GroupStartCoordinate = NewListItem.DiagonalIndex + NewListItem.InitiallIndexInDiagonal - SearchStringLength + 1;
                                NewListItem.GroupEndCoordinate = NewListItem.GroupStartCoordinate + NewListItem.GroupSize - 1;

                                NewListItem.NumberOfWordOfSearchString = WordsTableRowSearchString.NumberOfWord;
                                NewListItem.StartNumberInSearchStringWord = WordsTableRowSearchString.NumberInWord;
                                NewListItem.WordSizeOfSearchString = WordsTableRowSearchString.SizeOfWord;

                                NewListItem.ThisIsInitialGroupOfSearchStringWord = ThisIsInitialGroupOfSearchStringWord;

                                NewListItem.NumberOfWordOfDataString = WordsTableRowDataString.NumberOfWord;
                                NewListItem.StartNumberInDataStringWord = WordsTableRowDataString.NumberInWord;
                                NewListItem.WordSizeOfDataString = WordsTableRowDataString.SizeOfWord;

                                NewListItem.ThisIsInitialGroupOfDataStringWord = ThisIsInitialGroupOfDataStringWord;

                                if (Opt_ConsiderCoefficientOfCoveregeWordByGroup)
                                {
                                    NewListItem.CoverageRatioOfWordSearchString = (double)CurrentGroupSize / NewListItem.WordSizeOfSearchString;
                                    NewListItem.CoverageRatioOfWordDataString = (double)CurrentGroupSize / NewListItem.WordSizeOfDataString;
                                }

                                GroupsTable.Add(NewListItem);

                                if (Opt_OverGroups == true)
                                {

                                    if (CurrentNumberOfWordsOfSearchString == 0)
                                    {
                                        CurrentNumberOfWordsOfSearchString = NewListItem.NumberOfWordOfSearchString;
                                        CurrentNumberOfWordsOfDataString = NewListItem.NumberOfWordOfDataString;

                                        OverGroupSize += CurrentGroupSize;

                                        OverGroupsIndexes.Push(GroupsTable.IndexOf(NewListItem));
                                    }
                                    else if (
                                            CurrentNumberOfWordsOfSearchString == NewListItem.NumberOfWordOfSearchString
                                            && CurrentNumberOfWordsOfDataString == NewListItem.NumberOfWordOfDataString
                                        )
                                    {

                                        OverGroupSize += CurrentGroupSize;
                                        OverGroupsIndexes.Push(GroupsTable.IndexOf(NewListItem));
                                    }
                                    else
                                    {
                                        if (OverGroupsIndexes.Count() > 1)
                                        {
                                            foreach (int GroupTableIndex in OverGroupsIndexes)
                                            {
                                                var GroupTableRow = GroupsTable[GroupTableIndex];
                                                GroupTableRow.OverGroupSize = OverGroupSize;
                                                GroupTableRow.IncludedInOverGroup = true;

                                                if (Opt_ConsiderCoefficientOfCoveregeWordByGroup)
                                                {
                                                    GroupTableRow.CoverageRatioOfWordSearchString = (double)OverGroupSize / GroupTableRow.WordSizeOfSearchString;
                                                    GroupTableRow.CoverageRatioOfWordDataString = (double)OverGroupSize / GroupTableRow.WordSizeOfDataString;
                                                }

                                            }

                                        }

                                        OverGroupsIndexes.Clear();
                                        OverGroupsIndexes.Push(GroupsTable.IndexOf(NewListItem));
                                        OverGroupSize = CurrentGroupSize;

                                        CurrentNumberOfWordsOfSearchString = NewListItem.NumberOfWordOfSearchString;
                                        CurrentNumberOfWordsOfDataString = NewListItem.NumberOfWordOfDataString;

                                    }
                                }

                            }

                            CurrentGroupSize = 0;

                        }
                        else
                        {
                            CurrentGroupSize = 0;
                        }

                    }

                }

                if (CurrentGroupSize >= Opt_MinGroupSize)
                {
                    var InitiallIndexInDiagonal = SearchStringLength - CurrentGroupSize;
                    var WordsTableRowSearchString = TableOfSearchStringWords[InitiallIndexInDiagonal];
                    var ThisIsInitialGroupOfSearchStringWord = WordsTableRowSearchString.NumberInWord == 1;

                    var WordsTableRowDataString = TableOfDataStringWords[DiagonalIndex + InitiallIndexInDiagonal - SearchStringLength + 1];
                    var ThisIsInitialGroupOfDataStringWord = WordsTableRowDataString.NumberInWord == 1;

                    if (Opt_ControlConformityAttributeInitialGroupWord == false || 
                        ThisIsInitialGroupOfSearchStringWord == ThisIsInitialGroupOfDataStringWord)
                    {

                        NewListItem = new GroupsTableRow();

                        NewListItem.DiagonalIndex = DiagonalIndex;
                        NewListItem.InitiallIndexInDiagonal = InitiallIndexInDiagonal;
                        NewListItem.FinalDiagonalIndex = SearchStringLength - 1;
                        NewListItem.GroupSize = CurrentGroupSize;
                        NewListItem.OverGroupSize = CurrentGroupSize;

                        NewListItem.GroupStartCoordinate = NewListItem.DiagonalIndex + NewListItem.InitiallIndexInDiagonal - SearchStringLength + 1;
                        NewListItem.GroupEndCoordinate = NewListItem.GroupStartCoordinate + NewListItem.GroupSize - 1;

                        NewListItem.NumberOfWordOfSearchString = WordsTableRowSearchString.NumberOfWord;
                        NewListItem.StartNumberInSearchStringWord = WordsTableRowSearchString.NumberInWord;
                        NewListItem.WordSizeOfSearchString = WordsTableRowSearchString.SizeOfWord;

                        NewListItem.ThisIsInitialGroupOfSearchStringWord = ThisIsInitialGroupOfSearchStringWord;

                        NewListItem.NumberOfWordOfDataString = WordsTableRowDataString.NumberOfWord;
                        NewListItem.StartNumberInDataStringWord = WordsTableRowDataString.NumberInWord;
                        NewListItem.WordSizeOfDataString = WordsTableRowDataString.SizeOfWord;

                        NewListItem.ThisIsInitialGroupOfDataStringWord = ThisIsInitialGroupOfDataStringWord;

                        if (Opt_ConsiderCoefficientOfCoveregeWordByGroup)
                        {
                            NewListItem.CoverageRatioOfWordSearchString = (double)CurrentGroupSize / NewListItem.WordSizeOfSearchString;
                            NewListItem.CoverageRatioOfWordDataString = (double)CurrentGroupSize / NewListItem.WordSizeOfDataString;
                        }

                        GroupsTable.Add(NewListItem);

                        if (Opt_OverGroups == true && OverGroupsIndexes.Count() > 0)
                        {

                            if (
                                        CurrentNumberOfWordsOfSearchString == NewListItem.NumberOfWordOfSearchString
                                        && CurrentNumberOfWordsOfDataString == NewListItem.NumberOfWordOfDataString
                                    )
                            {

                                OverGroupSize += CurrentGroupSize;
                                OverGroupsIndexes.Push(GroupsTable.IndexOf(NewListItem));

                            }
                        }
                    }
                }

                if (Opt_OverGroups == true && OverGroupsIndexes.Count() > 1)
                {
                    foreach (int GroupTableIndex in OverGroupsIndexes)
                    {
                        var GroupTableRow = GroupsTable[GroupTableIndex];
                        GroupTableRow.OverGroupSize = OverGroupSize;
                        GroupTableRow.IncludedInOverGroup = true;
 
                        if (Opt_ConsiderCoefficientOfCoveregeWordByGroup)
                        {
                            GroupTableRow.CoverageRatioOfWordSearchString = (double)OverGroupSize / GroupTableRow.WordSizeOfSearchString;
                            GroupTableRow.CoverageRatioOfWordDataString = (double)OverGroupSize / GroupTableRow.WordSizeOfDataString;
                        }

                    }

                }

            }

            LabelResultsExclusionMode:


            List<GroupsTableRow> GroupsTable_Source = new List<GroupsTableRow>();

            if (Opt_FormingFullRepresentationOfResult == true)
            {         
                if (ModeOfExclusionOfPreviouslyFoundResults == true)
                {

                    List<GroupsTableRow> DeletedRows = new List<GroupsTableRow>();

                    foreach (var CurrentGroup in ResultGroups)
                    {
                        if (CurrentGroup.DepthOfRecursion == DepthOfRecursion - 1)
                        {
                            DeletedRows.Add(CurrentGroup);
                        }
                    }

                    foreach (var CurrentGroup in DeletedRows)
                    {
                        GroupsTable.Remove(CurrentGroup);
                    }

                    if(GroupsTable.Count() == 0)
                    {
                        return 0;
                    }

                }

                foreach (var CurrentGroup in GroupsTable)
                {
                    GroupsTable_Source.Add(CurrentGroup);

                    CurrentGroup.Copy_GroupSize = CurrentGroup.GroupSize;
                    CurrentGroup.Copy_OverGroupSize = CurrentGroup.OverGroupSize;
                    CurrentGroup.Copy_CoverageRatioOfWordSearchString = CurrentGroup.CoverageRatioOfWordSearchString;
                    CurrentGroup.Copy_CoverageRatioOfWordDataString = CurrentGroup.CoverageRatioOfWordDataString;
                    CurrentGroup.Copy_StartNumberInSearchStringWord = CurrentGroup.StartNumberInSearchStringWord;
                    CurrentGroup.Copy_StartNumberInDataStringWord = CurrentGroup.StartNumberInDataStringWord;
                    CurrentGroup.Copy_GroupStartCoordinate = CurrentGroup.GroupStartCoordinate;
                    CurrentGroup.Copy_GroupEndCoordinate = CurrentGroup.GroupEndCoordinate;

                }

            }

            if (Opt_SortTableGroupOneTime == true)
            {

                if (Opt_ConsiderCoefficientOfCoveregeWordByGroup)
                {
                    GroupsTable = GroupsTable.OrderByDescending(x => x.OverGroupSize)
                    .ThenByDescending(x => x.GroupSize)
                    .ThenByDescending(x => x.CoverageRatioOfWordSearchString)
                    .ThenByDescending(x => x.CoverageRatioOfWordDataString)
                    .ToList();
                }
                else
                {
                    GroupsTable = GroupsTable.OrderByDescending(x => x.OverGroupSize)
                    .ThenByDescending(x => x.GroupSize)
                    .ToList();
                }

            }

            FormChainOfGroups(ref GroupsTable, ref ResultGroups, Mode, DataStringLength, DepthOfRecursion);

            if (Opt_DeleteGroupsOfWordWithoutStartSymbol || Opt_DeleteGroupsOfWordWithLowCoverege)
            {
                DeleteGroupsOfWordsOfDataStringWithoutFirstSymbolAndWithLowCoverage(ref ResultGroups, DepthOfRecursion);
            }

            double PreRelevance;
            PreRelevance = CalculateCoefficient(ref TableOfSearchStringWords, SearchStringLength, DataStringLength, ref ResultGroups, Mode, DepthOfRecursion);

            if (Opt_RecursiveCalculationOfSearchResult)
            {

                if (ModeOfExclusionOfPreviouslyFoundResults == false)
                {
                    Relevance = PreRelevance;
                }

                if (PreRelevance >= Opt_ThresholdOfCoefficientWhenRecursiveCalculation)
                {

                    foreach (var CurrentGroup in GroupsTable_Source)
                    {
                        {
                            CurrentGroup.GroupSize = CurrentGroup.Copy_GroupSize;
                            CurrentGroup.OverGroupSize = CurrentGroup.Copy_OverGroupSize;
                            CurrentGroup.CoverageRatioOfWordSearchString = CurrentGroup.Copy_CoverageRatioOfWordSearchString;
                            CurrentGroup.CoverageRatioOfWordDataString = CurrentGroup.Copy_CoverageRatioOfWordDataString;
                            CurrentGroup.StartNumberInSearchStringWord = CurrentGroup.Copy_StartNumberInSearchStringWord;
                            CurrentGroup.StartNumberInDataStringWord = CurrentGroup.Copy_StartNumberInDataStringWord;
                            CurrentGroup.GroupStartCoordinate = CurrentGroup.Copy_GroupStartCoordinate;
                            CurrentGroup.GroupEndCoordinate = CurrentGroup.Copy_GroupEndCoordinate;
                        }
                    }

                    CalculateRelevancePrivate
                        (
                            ref ResultGroups,
                            true,
                            GroupsTable_Source,
                            ++DepthOfRecursion,
                            TableOfSearchStringWords,
                            SearchStringLength,
                            DataStringLength,
                            DataString
                        );
                }

                else
                {

                    return 0;

                }
            }
            else
            {

                Relevance = PreRelevance;

            }

            if (Opt_FormingBriefRepresentationOfResult == true && ModeOfExclusionOfPreviouslyFoundResults == false)
                {

                Stack<int> BriefDisplayCharactersOfDataString = new Stack<int>();
                Stack<int> BriefFoundCharactersInDataString = new Stack<int>();
                Stack<int> BriefFoundCharactersInDataStringExtended = new Stack<int>();

                FormRepresentationsOfResult(
                    ref ResultGroups,
                    ref BriefDisplayCharactersOfDataString,
                    ref BriefFoundCharactersInDataString,
                    ref BriefFoundCharactersInDataStringExtended,
                    Opt_FormingFullRepresentationOfResult,
                    ModeOfExclusionOfPreviouslyFoundResults);
            }

            return 0;

        }

    }

}