using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.UI;
using Telerik.WinControls.UI.Localization;


namespace RupLoader
{
    public class MyTimePickerLocalizationProvider : RadTimePickerLocalizationProvider
    {
        public override string GetLocalizedString(string id)
        {
            switch (id)
            {
                case RadTimePickerStringId.HourHeaderText: return "Godziny";
                case RadTimePickerStringId.MinutesHeaderText: return "Minuty";
                case RadTimePickerStringId.CloseButtonText: return "Zamknij";
                default: return string.Empty;
            }
        }
    }


    public class PolishRadGridLocalizationProvider : RadGridLocalizationProvider
    {
        public override string GetLocalizedString(string id)
        {
            switch (id)
            {
                case RadGridStringId.AddNewRowString:
                    return "Dodaj nowy wiersz";
                case RadGridStringId.BestFitMenuItem:
                    return "Najlepsze dopasowanie";
                case RadGridStringId.ClearSortingMenuItem:
                    return "Wyczyść sortowanie";
                case RadGridStringId.ClearValueMenuItem:
                    return "Wyczyść wartość";
                case RadGridStringId.ColumnChooserFormCaption:
                    return "Wybór kolumn";
                case RadGridStringId.ColumnChooserFormMessage:
                    return "Przeciągnij tu z tabeli naglówek kolumny\naby usunąć ją\nz bieżącego widoku.";
                case RadGridStringId.ColumnChooserMenuItem:
                    return "Wybierz kolumny";
                case RadGridStringId.CompositeFilterFormErrorCaption:
                    return "Błąd filtru";
                case RadGridStringId.ConditionalFormattingBtnAdd:
                    return "Dodaj nową regułę";
                case RadGridStringId.ConditionalFormattingBtnApply:
                    return "Zastosuj";
                case RadGridStringId.ConditionalFormattingBtnCancel:
                    return "Anuluj";
                case RadGridStringId.ConditionalFormattingBtnOK:
                    return "OK";
                case RadGridStringId.ConditionalFormattingBtnRemove:
                    return "Usuń";
                case RadGridStringId.ConditionalFormattingCaption:
                    return "Menedżer reguł formatowania warunkowego";
                case RadGridStringId.ConditionalFormattingChkApplyToRow:
                    return "Zastosuj tą regułę do całego wiersza";
                case RadGridStringId.ConditionalFormattingChooseOne:
                    return "[Wybierz jedną z opcji]";
                case RadGridStringId.ConditionalFormattingContains:
                    return "zawiera [Wartość 1]";
                case RadGridStringId.ConditionalFormattingDoesNotContain:
                    return "nie zawiera [Wartość 1]";
                case RadGridStringId.ConditionalFormattingEndsWith:
                    return "kończy się na [Wartość 1]";
                case RadGridStringId.ConditionalFormattingEqualsTo:
                    return "jest równa [Wartość 1]";
                case RadGridStringId.ConditionalFormattingGrpConditions:
                    return "Reguły";
                case RadGridStringId.ConditionalFormattingGrpProperties:
                    return "Właściwośći reguły";
                case RadGridStringId.ConditionalFormattingIsBetween:
                    return "jest z przedzialu od [Wartość 1] do [Wartość 2]";
                case RadGridStringId.ConditionalFormattingIsGreaterThan:
                    return "jest większa niz [Wartość 1]";
                case RadGridStringId.ConditionalFormattingIsGreaterThanOrEqual:
                    return "jest wększa lub równa [Wartość 1]";
                case RadGridStringId.ConditionalFormattingIsLessThan:
                    return "jest mniejsza niż [Wartość 1]";
                case RadGridStringId.ConditionalFormattingIsLessThanOrEqual:
                    return "jest mniejsza lub równe [Wartość 1]";
                case RadGridStringId.ConditionalFormattingIsNotBetween:
                    return "jest spoza przedziału od [Wartość 1] do [Wartość 2]";
                case RadGridStringId.ConditionalFormattingIsNotEqualTo:
                    return "jest rózna od [Wartość 1]";
                case RadGridStringId.ConditionalFormattingLblColumn:
                    return "Formatuj jedynie komórki, które";
                case RadGridStringId.ConditionalFormattingLblName:
                    return "Nazwa reguly:";
                case RadGridStringId.ConditionalFormattingLblType:
                    return "Wartość:";
                case RadGridStringId.ConditionalFormattingLblValue1:
                    return "Wartość 1:";
                case RadGridStringId.ConditionalFormattingLblValue2:
                    return "Wartość 2:";
                case RadGridStringId.ConditionalFormattingMenuItem:
                    return "Formatowanie warunkowe";
                case RadGridStringId.ConditionalFormattingRuleAppliesOn:
                    return "Reguła jest stosowana dla:";
                case RadGridStringId.ConditionalFormattingStartsWith:
                    return "zaczyna się od [Wartość 1]";
                case RadGridStringId.CopyMenuItem:
                    return "Kopiuj";
                case RadGridStringId.CustomFilterDialogBtnCancel:
                    return "Anuluj";
                case RadGridStringId.CustomFilterDialogBtnOk:
                    return "OK";
                case RadGridStringId.CustomFilterDialogCaption:
                    return "Własny filtr";
                case RadGridStringId.CustomFilterDialogCheckBoxNot:
                    return "Nie";
                case RadGridStringId.CustomFilterDialogFalse:
                    return "Fałsz";
                case RadGridStringId.CustomFilterDialogLabel:
                    return "Pokazuj wiersze, dla których:";
                case RadGridStringId.CustomFilterDialogRbAnd:
                    return "Oraz";
                case RadGridStringId.CustomFilterDialogRbOr:
                    return "Lub";
                case RadGridStringId.CustomFilterDialogTrue:
                    return "Prawda";
                case RadGridStringId.CustomFilterMenuItem:
                    return "Własny filtr";
                case RadGridStringId.DeleteRowMenuItem:
                    return "Usuń wiersz";
                case RadGridStringId.EditMenuItem:
                    return "Edytuj";
                case RadGridStringId.FilterCompositeNotOperator:
                    return "NIE";
                case RadGridStringId.FilterFunctionBetween:
                    return "Pomiędzy";
                case RadGridStringId.FilterFunctionContains:
                    return "Zawiera";
                case RadGridStringId.FilterFunctionCustom:
                    return "Własny";
                case RadGridStringId.FilterFunctionDoesNotContain:
                    return "Nie zawiera";
                case RadGridStringId.FilterFunctionEndsWith:
                    return "Kończy się na";
                case RadGridStringId.FilterFunctionEqualTo:
                    return "Jest równe";
                case RadGridStringId.FilterFunctionGreaterThan:
                    return "Jest większe niż";
                case RadGridStringId.FilterFunctionGreaterThanOrEqualTo:
                    return "Jest większe lub równe";
                case RadGridStringId.FilterFunctionIsEmpty:
                    return "Jest puste";
                case RadGridStringId.FilterFunctionIsNull:
                    return "Jest równe NULL";
                case RadGridStringId.FilterFunctionLessThan:
                    return "Jest mniejsze niż";
                case RadGridStringId.FilterFunctionLessThanOrEqualTo:
                    return "Jest mniejsze lub równe";
                case RadGridStringId.FilterFunctionNoFilter:
                    return "Bez filtrowania";
                case RadGridStringId.FilterFunctionNotBetween:
                    return "Jest spoza zakresu";
                case RadGridStringId.FilterFunctionNotEqualTo:
                    return "Jest rózne od";
                case RadGridStringId.FilterFunctionNotIsEmpty:
                    return "Jest niepuste";
                case RadGridStringId.FilterFunctionNotIsNull:
                    return "Jest różne od NULL";
                case RadGridStringId.FilterFunctionStartsWith:
                    return "Zaczyna się od";
                case RadGridStringId.FilterLogicalOperatorAnd:
                    return "ORAZ";
                case RadGridStringId.FilterLogicalOperatorOr:
                    return "LUB";
                case RadGridStringId.FilterMenuAvailableFilters:
                    return "Dostepne filtry";
                case RadGridStringId.FilterMenuButtonCancel:
                    return "Anuluj";
                case RadGridStringId.FilterMenuButtonOK:
                    return "OK";
                case RadGridStringId.FilterMenuClearFilters:
                    return "Wyczyść filtry";
                case RadGridStringId.FilterMenuSearchBoxText:
                    return "Szukaj...";
                case RadGridStringId.FilterMenuSelectionAll:
                    return "Wszystkie";
                case RadGridStringId.FilterMenuSelectionAllSearched:
                    return "Wszystkie wyniki wyszukiwania";
                case RadGridStringId.FilterMenuSelectionNotNull:
                    return "Różne od NULL";
                case RadGridStringId.FilterMenuSelectionNull:
                    return "Równe NULL";
                case RadGridStringId.FilterOperatorBetween:
                    return "Pomiędzy";
                case RadGridStringId.FilterOperatorContains:
                    return "Zawiera";
                case RadGridStringId.FilterOperatorCustom:
                    return "Własny filtr";
                case RadGridStringId.FilterOperatorDoesNotContain:
                    return "Nie zawiera";
                case RadGridStringId.FilterOperatorEndsWith:
                    return "Kończy się na";
                case RadGridStringId.FilterOperatorEqualTo:
                    return "Jest równe";
                case RadGridStringId.FilterOperatorGreaterThan:
                    return "Jest większe niż";
                case RadGridStringId.FilterOperatorGreaterThanOrEqualTo:
                    return "Jest większe lub równe";
                case RadGridStringId.FilterOperatorIsContainedIn:
                    return "Zawiera się w";
                case RadGridStringId.FilterOperatorIsEmpty:
                    return "Jest puste";
                case RadGridStringId.FilterOperatorIsLike:
                    return "Wygląda jak";
                case RadGridStringId.FilterOperatorIsNull:
                    return "Jest równe NULL";
                case RadGridStringId.FilterOperatorLessThan:
                    return "Jest mniejsze niż";
                case RadGridStringId.FilterOperatorLessThanOrEqualTo:
                    return "Jest mniejsze lub równe";
                case RadGridStringId.FilterOperatorNoFilter:
                    return "Brak filtru";
                case RadGridStringId.FilterOperatorNotBetween:
                    return "Nie zawiera się w przedziale";
                case RadGridStringId.FilterOperatorNotEqualTo:
                    return "Jest różne od";
                case RadGridStringId.FilterOperatorNotIsContainedIn:
                    return "Nie zawiera się w";
                case RadGridStringId.FilterOperatorNotIsEmpty:
                    return "Jest niepuste";
                case RadGridStringId.FilterOperatorNotIsLike:
                    return "Nie wygląda jak";
                case RadGridStringId.FilterOperatorNotIsNull:
                    return "Jest różne od NULL";
                case RadGridStringId.FilterOperatorStartsWith:
                    return "Zaczyna się od";
                case RadGridStringId.GroupByThisColumnMenuItem:
                    return "Grupuj wedlug tej kolumny";
                case RadGridStringId.GroupingPanelDefaultMessage:
                    return "Przeciągnij tu kolumne aby pogrupować";
                case RadGridStringId.GroupingPanelHeader:
                    return "Grupuj według:";
                case RadGridStringId.HideMenuItem:
                    return "Ukryj kolumnę";
                case RadGridStringId.NoDataText:
                    return "Brak danych do wyświetlenia";
                case RadGridStringId.PasteMenuItem:
                    return "Wklej";
                case RadGridStringId.PinAtBottomMenuItem:
                    return "Przypnij na dole";
                case RadGridStringId.PinAtLeftMenuItem:
                    return "Przypnij po lewej";
                case RadGridStringId.PinAtRightMenuItem:
                    return "Przypnij po prawej";
                case RadGridStringId.PinAtTopMenuItem:
                    return "Przypnij na górze";
                case RadGridStringId.PinMenuItem:
                    return "Przypnij";
                case RadGridStringId.SortAscendingMenuItem:
                    return "Sortuj rosnąco";
                case RadGridStringId.SortDescendingMenuItem:
                    return "Sortuj malejąco";
                case RadGridStringId.UngroupThisColumn:
                    return "Rozgrupuj tą kolumnę";
                case RadGridStringId.UnpinMenuItem:
                    return "Odepnij kolumnę";
                case RadGridStringId.UnpinRowMenuItem:
                    return "Odepnij wiersz";
            }

            return string.Empty;
        }
    }
}