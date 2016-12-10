DECLARE @GeneralModDefIDView uniqueidentifier
DECLARE @FriendlyNameView nvarchar(128)
DECLARE @DesktopSrcView nvarchar(256)
DECLARE @MobileSrc nvarchar(256)
DECLARE @AssemblyName varchar(50)
DECLARE @ClassName nvarchar(128)
DECLARE @AdminView bit
DECLARE @Searchable bit

SET @GeneralModDefIDView = NEWID()   -- Or if you want full control: = '{12784E32-688A-4B8A-87C4-34108BF12DAA}'
SET @FriendlyNameView =  'Appleseed Search' -- You enter the module UI name here
SET @DesktopSrcView = 'DesktopModules\CustomModules\Nate\Search\CollectionIndexSearch.ascx'-- You enter actual filename here
SET @MobileSrc = ''
SET @AssemblyName = 'Appleseed.Services.Search.Web.dll'
SET @ClassName = 'Appleseed.Services.Search.Web.DesktopModules.CustomModules.Anant.Search.CollectionIndexSearch'
SET @AdminView = 0
SET @Searchable = 0

IF NOT EXISTS (SELECT DesktopSrc FROM rb_GeneralModuleDefinitions WHERE DesktopSrc = @DesktopSrcView)
BEGIN
	-- Installs module - View
	EXEC [rb_AddGeneralModuleDefinitions] @GeneralModDefIDView, @FriendlyNameView, @DesktopSrcView, @MobileSrc, @AssemblyName, @ClassName, @AdminView, @Searchable

	-- Install it for default portal
	EXEC [rb_UpdateModuleDefinitions] @GeneralModDefIDView, 0, 1	
END
