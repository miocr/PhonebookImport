//------------------------------------------------------------------------------
// <auto-generated>
//     Tento kód byl generován nástrojem.
//     Verze modulu runtime:4.0.30319.42000
//
//     Změny tohoto souboru mohou způsobit nesprávné chování a budou ztraceny,
//     dojde-li k novému generování kódu.
// </auto-generated>
//------------------------------------------------------------------------------

[assembly: System.Data.Mapping.EntityViewGenerationAttribute(typeof(Edm_EntityMappingGeneratedViews.ViewsForBaseEntitySetsB7243FA130C2FCE3AD9245C99EAEF99475AD67D4E891A7A192998C16FDD72E2E))]

namespace Edm_EntityMappingGeneratedViews
{
    
    
    /// <Summary>
    /// Typ obsahuje zobrazení pro objekty EntitySets a AssociationSets generované v době návrhu.
    /// </Summary>
    public sealed class ViewsForBaseEntitySetsB7243FA130C2FCE3AD9245C99EAEF99475AD67D4E891A7A192998C16FDD72E2E : System.Data.Mapping.EntityViewContainer
    {
        
        /// <Summary>
        /// Konstruktor ukládá zobrazení pro rozsahy a také generované hodnoty hash na základě zobrazení a uzavření metadat a mapování.
        /// </Summary>
        public ViewsForBaseEntitySetsB7243FA130C2FCE3AD9245C99EAEF99475AD67D4E891A7A192998C16FDD72E2E()
        {
            this.EdmEntityContainerName = "Phonebook";
            this.StoreEntityContainerName = "PhonebookImportServerPhonebookModelStoreContainer";
            this.HashOverMappingClosure = "905310abcf6dbe4100742ed303510989b6e1445edf350faec26fe0476f9badbf";
            this.HashOverAllExtentViews = "357510909d6422cfd8c0bad209bb52088ef23ea7c9c9f4c578eea6200b73a1aa";
            this.ViewCount = 2;
        }
        
        /// <Summary>
        /// Metoda vrací zobrazení pro daný index.
        /// </Summary>
        protected override System.Collections.Generic.KeyValuePair<string, string> GetViewAt(int index)
        {
            if ((index == 0))
            {
                return GetView0();
            }
            if ((index == 1))
            {
                return GetView1();
            }
            throw new System.IndexOutOfRangeException();
        }
        
        /// <Summary>
        /// návratové zobrazení pro PhonebookImportServerPhonebookModelStoreContainer.phonebook
        /// </Summary>
        private System.Collections.Generic.KeyValuePair<string, string> GetView0()
        {
            return new System.Collections.Generic.KeyValuePair<string, string>("PhonebookImportServerPhonebookModelStoreContainer.phonebook", @"
    SELECT VALUE -- Constructing phonebook
        [PhonebookImportServer.PhonebookModel.Store.phonebook](T1.phonebook_id, T1.[phonebook.sys_country_id], T1.phonebook_number, T1.phonebook_description, T1.phonebook_company, T1.[phonebook.phone_type], T1.phonebook_public, T1.phonebook_vip)
    FROM (
        SELECT 
            T.id AS phonebook_id, 
            T.sys_country_id AS [phonebook.sys_country_id], 
            T.number AS phonebook_number, 
            T.description AS phonebook_description, 
            T.company AS phonebook_company, 
            T.phone_type AS [phonebook.phone_type], 
            T.public AS phonebook_public, 
            T.vip AS phonebook_vip, 
            True AS _from0
        FROM Phonebook.phonebook AS T
    ) AS T1");
        }
        
        /// <Summary>
        /// návratové zobrazení pro Phonebook.phonebook
        /// </Summary>
        private System.Collections.Generic.KeyValuePair<string, string> GetView1()
        {
            return new System.Collections.Generic.KeyValuePair<string, string>("Phonebook.phonebook", @"
    SELECT VALUE -- Constructing phonebook
        [PhonebookImportServer.PhonebookModel.phonebook](T1.phonebook_id, T1.[phonebook.sys_country_id], T1.phonebook_number, T1.phonebook_description, T1.phonebook_company, T1.[phonebook.phone_type], T1.phonebook_public, T1.phonebook_vip)
    FROM (
        SELECT 
            T.id AS phonebook_id, 
            T.sys_country_id AS [phonebook.sys_country_id], 
            T.number AS phonebook_number, 
            T.description AS phonebook_description, 
            T.company AS phonebook_company, 
            T.phone_type AS [phonebook.phone_type], 
            T.public AS phonebook_public, 
            T.vip AS phonebook_vip, 
            True AS _from0
        FROM PhonebookImportServerPhonebookModelStoreContainer.phonebook AS T
    ) AS T1");
        }
    }
}
