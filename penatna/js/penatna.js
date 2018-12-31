/* Penatna */

function formatDate(dateTime)
{
	if (dateTime === null) {
		return "";
	}
	var formattedDate = new Date(dateTime);
	var d = formattedDate.getDate();
	var m = formattedDate.getMonth();
	m += 1;  // JavaScript months are 0-11
	var y = formattedDate.getFullYear();
	return m + "/" + d + "/" + y;
}
