
![Wordify](https://imgur.com/rc8Wz56.png)

# Introduction 
 Handwritting is still very useful in today's technology centric life style. Gives you the freedom to write where you want
on a page and easily switch where on the page you want your next thought to go, and there'sthe benifit of the muscle memory you gain
from writing the characters vs just clicking buttons. But paper documents can get cluttered, take up real space, and be frustrating to organize.
 Wordify is an application designed to help users convert handwritten documents, be it class notes, 
old family recipes, you name it. With this app you can take a photo of your document and scan it with our app to product machine readable text.
You can edit and adjust it as you see fit, and save the image and text into our database where it is easily organized and referencable for the users convenience. 


## Data Flow 
![Data Flow](Wordify/Assets/WordifyDataFlow.PNG)  

## Wireframe
![Wireframe](Wordify/Assets/WordifyWireframe.jpg)  

## Database Schema
![Database Schema](Wordify/Assets/WordifySchema.PNG)  
Our Schema includes the tables that we will be using in the next stage of our application. Currently only the User and Notes tables are being used. Users and Notes are being stored in their own SQL Databases, while the Images and their Text are stored in Azure Blob Storage. The User table stores confirms the current users identity, and is the reference point for the Notes. The Notes table stores references to the User who made it, and the Image and Text stored in the Blob Storage. Additional information includes a Title for the Note and the Date when the Note was created.  

## Happy Path

Have a jpg, png or bmp file saved on your computer. Hit the "Browse" button and select the image you want, then hit "Upload."
You will see your image and what the OCR was able to decipher off of it in an area that you can edit. From there, if you are using 
the site anonymously you can copy + paste the text area and save it onto your own document. If you are logged in, you can then title it and 
save it into our database. From the Profile page you can view, edit, or delete all images you have uploaded as you see fit. 


## Technologies used
Microsoft Cognitive Services OCR(Optical Character Reader) Handwriting Mode, Visual Studio, VSTS, Azure, Azure Blob Storage, Microsoft Identity Framework

C#, HTML5, CSS3, Razor Pages, SQL 

# Contributions
Assistence from the Instructional Staff of Code Fellows, as well as Microsoft Docs

