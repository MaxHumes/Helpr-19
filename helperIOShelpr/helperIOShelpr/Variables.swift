//
//  Variables.swift
//  helperIOShelpr
//
//  Created by Sam Horn on 4/11/20.
//  Copyright © 2020 Sam Horn. All rights reserved.
//

import Foundation
import UIKit

var token: Data? = nil

struct Thread: Codable {
    var thread_id: Int
    var name: String
    var description: String

}

struct Post: Codable {
    var post_id: Int
    var thread_id: Int
    var user_id: Int
    var name: String
    var description: String
}

class ThreadCell: UITableViewCell {
    
    var thread: Thread?
    @IBOutlet weak var threadName: UILabel!
}

class PostCell: UITableViewCell {
    
    var post: Post?
   
    
}
