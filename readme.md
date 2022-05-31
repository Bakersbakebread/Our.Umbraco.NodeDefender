
## Usage

To setup the NodeDefender, simply add a section to your `appsettings.json` called `NodeDefender`.


```
  "NodeDefender": {
    "AllowedUserGroups": [],
    "DenyRename": {},
    "DenyDuplicate": {},
    "DenyDelete": {}
  }
```

### AllowedUserGroups
These are string aliases to the user groups of the users who are immune to the defender, remember, with great power comes great responsibility.

### DenyOptions
DenyRename, DenyDuplicate and DenyDelete all share the same structure
```
{
  // an array of document string aliases
  "DoctypeAliases": [ 
    "homePage"
  ],
  // an array of specific node Ids
  "NodeIds": [
    1234
  ],
  // The error message to send
  "Message": {
    "Category": "Error",
    "Message": "You are not allowed to rename this node.",
    "Type": "Error" // Must be 'Error', 'Info', 'Warning' or 'Success'
  }
}
```

### Full Example
```
  "NodeDefender": {
    "AllowedUserGroups": [
      "defenderImmune"
    ],
    "DenyRename": {
      "DoctypeAliases": [
        "homePage"
      ],
      "NodeIds": [
        1234
      ],
      "Message": {
        "Category": "Error",
        "Message": "You are not allowed to rename this node.",
        "Type": "Error"
      }
    },
    "DenyDuplicate": {
      "DoctypeAliases": [
        "homePage"
      ],
      "NodeIds": [
        1234
      ]
    },
    "DenyDelete": {
      "DoctypeAliases": [
        "homePage"
      ],
      "NodeIds": [
        1234
      ]
    }
  }
  ```

