# INI

Command Line INI Editor

## Installation

This utility is made for portable usage,
for example to fix wrong paths in an INI file to keep an application working in a portable fashion.

No installation is needed. Go to the "Releases" section and download the latest Release.

## How To Use

Command line arguments: `ini.exe /G|/S|/L <Filename> [Section [Key [Value]]]`

	/G        - Get the value of the given 'key' in the given 'Section'
	/S        - Set the value of the given 'Key' in the given 'Section' to 'Value'
	/L        - Lists all ini values in 'Section.Key=Value' format

	The mode should be specified first

	Filename  - File to read/write. Created in 'set' mode if it doesn't exists. Must be specified before the ini Section/Key/Value argument

	Section   - INI Section
				get:  (required) Section to read
				set:  (required) Section to search key in
				list: (optional) Section to list values of

	Key       - Key of the section
				get:  (required) key value to read
				set:  (required) key value to set
				list: Not supported

	Value     - Value to write to the given key
				get:  Not supported
				set:  (optional) Value to write to the key. If missing, the key is deleted
				list: Not supported

## Notable Features

- Read and write individual keys
- List entire file contents or individual sections
- File created if needed
- Keys are Case Sensitive
- Deletes sections if empty

## Comments

Comments in the INI file will be erased when any key is set