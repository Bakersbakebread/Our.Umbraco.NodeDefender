
# Our.Umbraco.NodeDefender

A no-nonsense approach to defending nodes. I often find myself writing the same thing in a lot of projects to prevent editors from renaming a settings node, or moving it to the recycle bin. This package aims to solve that simply.



The package has no dependencies as is configured in `appsettings.json` thus making it only usable on Umbraco 9+




## Features

- Prevent the duplication of nodes
- Prevent the renaming of nodes 
- Prevent the deletion of nodes


## Installation


    
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

