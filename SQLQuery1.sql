


update RolesPermissionXml set xmlInfo =  CONVERT(XML, N'<UserPermisionsRole permision="Allowed">
	<Administrador idValue="1">
		<allowed>ALL</allowed>
	</Administrador>
	<Reponedor idValue="2">
		<allowed>INDEX</allowed>
		<allowed>PRODUCTS</allowed>
		<allowed>STOCKS</allowed>
		<allowed>PRODUCTS_EDITIONS</allowed>
		<allowed>CHANGE</allowed>
    <allowed>CHANGEPASSWORD</allowed>
		<allowed>CLOSESESSION</allowed>
	</Reponedor>
	<Auditor idValue="3">
		<allowed>INDEX</allowed>
		<allowed>STATS</allowed>
    <allowed>CHANGEPASSWORD</allowed>
		<allowed>CLOSESESSION</allowed>
	</Auditor>
	<Control idValue="4">
		<allowed>INDEX</allowed>
		<allowed>GET_USERS</allowed>
    <allowed>CHANGEPASSWORD</allowed>
		<allowed>SET_MACHINE_ADMIN</allowed>
		<allowed>CLOSESESSION</allowed>
	</Control>
</UserPermisionsRole>', 2) where IdXml = 1;