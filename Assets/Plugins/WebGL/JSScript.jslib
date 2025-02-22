mergeInto(LibraryManager.library, {
    OpenFolderBrowser: async function () {
        try {
            let input = document.createElement('input');
            input.type = 'file';
            input.webkitdirectory = true;

            input.addEventListener('change', async function (event) {
                if (event.target.files.length > 0) {
                    let files = Array.from(event.target.files).filter(file => file.name.endsWith('.ply'));

                    SendMessage("ParameteresUI", "ReceivePlyFileCount", files.length);


                    // Demander à l'utilisateur de choisir un dossier
                    let destDirHandle = await window.showDirectoryPicker();

                    for (let file of files) {
                        let fileHandle = await destDirHandle.getFileHandle(file.name, { create: true });
                        let writable = await fileHandle.createWritable();

                        let arrayBuffer = await file.arrayBuffer();
                        await writable.write(arrayBuffer);
                        await writable.close();

                        // Envoyer uniquement le chemin du fichier à Unity
                        SendMessage("ParameteresUI", "ReceivePlyFilePath", file.name);
                    }
                }
            });

            document.body.appendChild(input);
            input.click();
            document.body.removeChild(input);
        } catch (error) {
            console.error("Erreur lors de la sélection du dossier :", error);
        }
    }
});
