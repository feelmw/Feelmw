const SPREADSHEETS = {
  Plangintza: '12znzweKUTKxMYufziFxza5uth37OX6wwq8PDK71cV0w',
  Logistika: '1Cjuuv9mY3sk3i27Fmi2ccjCl7rRWSpW_ou1KEGzE4Ng'
};

function doPost(e) {
  try {
    const request = JSON.parse(e.postData.contents || '{}');
    const spreadsheetId = SPREADSHEETS[request.workbook];
    if (!spreadsheetId) {
      throw new Error('Workbook ezezaguna.');
    }

    const sheet = SpreadsheetApp.openById(spreadsheetId).getSheetByName(request.sheet);
    if (!sheet) {
      throw new Error('Orria ez da aurkitu.');
    }

    switch ((request.action || '').toLowerCase()) {
      case 'read':
        return json({ success: true, rows: readRows(sheet) });
      case 'save':
        saveRows(sheet, request.rows || []);
        return json({ success: true });
      case 'add':
        sheet.appendRow(request.row || []);
        return json({ success: true });
      case 'update':
        updateRow(sheet, request.rowIndex, request.row || []);
        return json({ success: true });
      case 'delete':
        deleteRow(sheet, request.rowIndex);
        return json({ success: true });
      default:
        throw new Error('Ekintza ezezaguna.');
    }
  } catch (error) {
    return json({ success: false, message: String(error && error.message ? error.message : error) });
  }
}

function readRows(sheet) {
  const lastRow = sheet.getLastRow();
  const lastColumn = sheet.getLastColumn();
  if (lastRow === 0 || lastColumn === 0) {
    return [];
  }

  return sheet.getRange(1, 1, lastRow, lastColumn).getDisplayValues();
}

function saveRows(sheet, rows) {
  sheet.clearContents();
  if (!rows.length) {
    return;
  }

  const width = rows.reduce((max, row) => Math.max(max, row.length), 0);
  const normalized = rows.map(row => {
    const copy = row.slice();
    while (copy.length < width) {
      copy.push('');
    }
    return copy;
  });

  sheet.getRange(1, 1, normalized.length, width).setValues(normalized);
}

function updateRow(sheet, rowIndex, row) {
  const sheetRow = Number(rowIndex) + 2;
  if (!Number.isFinite(sheetRow) || sheetRow < 2) {
    throw new Error('Errenkada baliogabea.');
  }

  sheet.getRange(sheetRow, 1, 1, row.length).setValues([row]);
}

function deleteRow(sheet, rowIndex) {
  const sheetRow = Number(rowIndex) + 2;
  if (!Number.isFinite(sheetRow) || sheetRow < 2) {
    throw new Error('Errenkada baliogabea.');
  }

  sheet.deleteRow(sheetRow);
}

function json(payload) {
  return ContentService
    .createTextOutput(JSON.stringify(payload))
    .setMimeType(ContentService.MimeType.JSON);
}
